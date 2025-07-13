using System.Net;
using AutoMapper;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResNet.Domain.Constants;
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;

namespace Infrastructure.Services;

public class RestaurantService(
    DataContext context,
    IMapper mapper,
    ILogger<RestaurantService> logger
) : IRestaurantService
{
    public async Task<PagedResponse<List<GetRestaurantDto>>> GetAllRestaurantsAsync(RestaurantFilter filter)
    {
        logger.LogInformation("GetAllRestaurantsAsync called with filter {@Filter}", filter);

        IQueryable<Restaurant> query = context.Restaurants.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(r => r.Name.ToLower().Contains(filter.Name.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.Cuisine))
            query = query.Where(r => r.Cuisine != null && r.Cuisine.ToLower().Contains(filter.Cuisine.ToLower()));

        if (filter.MinRating.HasValue)
            query = query.Where(r => r.Rating >= filter.MinRating.Value);

        if (filter.MaxRating.HasValue)
            query = query.Where(r => r.Rating <= filter.MaxRating.Value);

        int totalCount = await query.CountAsync();

        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        var restaurants = await query
            .OrderBy(r => r.Name)
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetRestaurantDto>>(restaurants);

        return new PagedResponse<List<GetRestaurantDto>>(dtos, validFilter.PageNumber, validFilter.PageSize, totalCount);
    }

    public async Task<Response<GetRestaurantDto>> GetRestaurantByIdAsync(int id)
    {
        logger.LogInformation("GetRestaurantByIdAsync called with id={Id}", id);

        var restaurant = await context.Restaurants
            .Include(r => r.Tables)
            .Include(r => r.Menu)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (restaurant == null)
        {
            logger.LogWarning("Restaurant with id={Id} not found", id);
            return new Response<GetRestaurantDto>(HttpStatusCode.NotFound, "Restaurant not found");
        }

        var categories = await context.Categories
            .Where(c => c.Products.Any(p => p.RestaurantId == id))
            .AsNoTracking()
            .ToListAsync();

        var dto = mapper.Map<GetRestaurantDto>(restaurant);
        dto.Categories = mapper.Map<List<GetCategoryDto>>(categories);

        return Response<GetRestaurantDto>.Success(dto);
    }

    public async Task<Response<GetRestaurantDto>> AddRestaurantAsync(CreateRestaurantDto restaurantDto)
    {
        logger.LogInformation("AddRestaurantAsync called");

        var exists = await context.Restaurants.AnyAsync(r => r.Name.ToLower() == restaurantDto.Name.ToLower());
        if (exists)
            return new Response<GetRestaurantDto>(HttpStatusCode.BadRequest, "Restaurant name is already taken");

        var restaurant = mapper.Map<Restaurant>(restaurantDto);

        await context.Restaurants.AddAsync(restaurant);
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetRestaurantDto>(HttpStatusCode.BadRequest, "Restaurant not created");

        var dto = mapper.Map<GetRestaurantDto>(restaurant);
        return Response<GetRestaurantDto>.Success(dto);
    }

    public async Task<Response<GetRestaurantDto>> UpdateRestaurantAsync(int id, UpdateRestaurantDto restaurantDto)
    {
        logger.LogInformation("UpdateRestaurantAsync called with id={Id}", id);

        var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
        if (restaurant == null)
            return new Response<GetRestaurantDto>(HttpStatusCode.NotFound, "Restaurant not found");

        if (!string.Equals(restaurant.Name, restaurantDto.Name, StringComparison.OrdinalIgnoreCase))
        {
            var nameTaken = await context.Restaurants.AnyAsync(r => r.Name.ToLower() == restaurantDto.Name.ToLower() && r.Id != id);
            if (nameTaken)
                return new Response<GetRestaurantDto>(HttpStatusCode.BadRequest, "Restaurant name is already taken");
        }

        restaurant.Name = restaurantDto.Name;
        restaurant.Description = restaurantDto.Description;
        restaurant.Cuisine = restaurantDto.Cuisine;
        restaurant.Address = restaurantDto.Address;
        restaurant.Phone = restaurantDto.Phone;
        restaurant.Rating = restaurant.Rating;
        restaurant.ImageUrl = restaurant.ImageUrl;
        restaurant.OpeningHours = restaurantDto.OpeningHours;

        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetRestaurantDto>(HttpStatusCode.BadRequest, "Restaurant not updated");

        var dto = mapper.Map<GetRestaurantDto>(restaurant);
        return Response<GetRestaurantDto>.Success(dto);
    }

    public async Task<Response<string>> DeleteRestaurantAsync(int id)
    {
        logger.LogInformation("DeleteRestaurantAsync called with id={Id}", id);

        var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
        if (restaurant == null)
            return new Response<string>(HttpStatusCode.NotFound, "Restaurant not found");

        context.Restaurants.Remove(restaurant);
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Restaurant not deleted");

        return Response<string>.Success("Restaurant deleted successfully");
    }

    public async Task<Response<List<GetProductDto>>> GetMenuByRestaurantIdAsync(int restaurantId)
    {
        logger.LogInformation("GetMenuByRestaurantIdAsync called with restaurantId={Id}", restaurantId);

        var products = await context.Products
            .Where(p => p.RestaurantId == restaurantId)
            .AsNoTracking()
            .ToListAsync();

        var dtos = mapper.Map<List<GetProductDto>>(products);
        return Response<List<GetProductDto>>.Success(dtos);
    }

    public async Task<Response<List<GetTableDto>>> GetAvailableTablesAsync(int restaurantId, DateTime? dateTime)
    {
        logger.LogInformation("GetAvailableTablesAsync called with restaurantId={Id}, dateTime={DateTime}", restaurantId, dateTime);

        var tables = await context.Tables
            .Where(t => t.RestaurantId == restaurantId)
            .AsNoTracking()
            .ToListAsync();

        if (dateTime.HasValue)
        {
            var bookingDuration = TimeSpan.FromHours(2);
            var targetStart = dateTime.Value;
            var targetEnd = targetStart.Add(bookingDuration);

            var bookedTableIds = await context.Bookings
                .Where(b => b.Table.RestaurantId == restaurantId
                            && b.Status != BookingStatus.Cancelled
                            && (
                                (b.BookingTime >= targetStart && b.BookingTime < targetEnd) ||
                                (b.BookingTime.AddHours(2) > targetStart && b.BookingTime.AddHours(2) <= targetEnd) ||
                                (b.BookingTime <= targetStart && b.BookingTime.AddHours(2) >= targetEnd)
                               )
                       )
                .Select(b => b.TableId)
                .ToListAsync();

            tables = tables.Where(t => !bookedTableIds.Contains(t.Id)).ToList();
        }

        var dtos = mapper.Map<List<GetTableDto>>(tables);
        return Response<List<GetTableDto>>.Success(dtos);
    }
}
