using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResNet.Domain.Constants;
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;
using ResNet.Domain.Filters;

namespace Infrastructure.Services;

public class TableService(
    DataContext context,
    IMapper mapper,
    ILogger<TableService> logger
) : ITableService
{
    private const int DefaultBookingDurationHours = 2;

    public async Task<PagedResponse<List<GetTableDto>>> GetTablesByRestaurantIdAsync(int restaurantId, TableFilter filter)
    {
        logger.LogInformation("GetTablesByRestaurantIdAsync called with restaurantId={RestaurantId} and filter {@Filter}", restaurantId, filter);

        var query = context.Tables
            .Where(t => t.RestaurantId == restaurantId)
            .AsNoTracking();

        if (filter.Status != null)
            query = query.Where(t => t.Status == filter.Status);

        if (filter.MinSeats.HasValue)
            query = query.Where(t => t.Seats >= filter.MinSeats.Value);

        if (filter.MaxSeats.HasValue)
            query = query.Where(t => t.Seats <= filter.MaxSeats.Value);

        var totalCount = await query.CountAsync();

        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        var tables = await query
            .OrderBy(t => t.Id)
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetTableDto>>(tables);

        return new PagedResponse<List<GetTableDto>>(dtos, validFilter.PageNumber, validFilter.PageSize, totalCount);
    }

    public async Task<Response<GetTableDto>> GetTableByIdAsync(int id)
    {
        logger.LogInformation("GetTableByIdAsync called with id={Id}", id);

        var table = await context.Tables.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

        if (table == null)
        {
            logger.LogWarning("Table with id={Id} not found", id);
            return new Response<GetTableDto>(HttpStatusCode.NotFound, "Table not found");
        }

        var dto = mapper.Map<GetTableDto>(table);
        return Response<GetTableDto>.Success(dto);
    }

    public async Task<Response<GetTableDto>> AddTableAsync(CreateTableDto tableDto)
    {
        logger.LogInformation("AddTableAsync called");

        var restaurantExists = await context.Restaurants.AnyAsync(r => r.Id == tableDto.RestaurantId);
        if (!restaurantExists)
        {
            logger.LogWarning("AddTableAsync failed: Restaurant with id={RestaurantId} not found", tableDto.RestaurantId);
            return new Response<GetTableDto>(HttpStatusCode.BadRequest, "Restaurant not found");
        }

        var table = mapper.Map<Table>(tableDto);
        await context.Tables.AddAsync(table);
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetTableDto>(HttpStatusCode.BadRequest, "Table not created");

        var dto = mapper.Map<GetTableDto>(table);
        return Response<GetTableDto>.Success(dto);
    }

    public async Task<Response<GetTableDto>> UpdateTableAsync(int id, UpdateTableDto tableDto)
    {
        logger.LogInformation("UpdateTableAsync called with id={Id}", id);

        var table = await context.Tables.FirstOrDefaultAsync(t => t.Id == id);
        if (table == null)
        {
            logger.LogWarning("Table with id={Id} not found", id);
            return new Response<GetTableDto>(HttpStatusCode.NotFound, "Table not found");
        }

        if (table.RestaurantId != tableDto.RestaurantId)
        {
            var restaurantExists = await context.Restaurants.AnyAsync(r => r.Id == tableDto.RestaurantId);
            if (!restaurantExists)
            {
                logger.LogWarning("UpdateTableAsync failed: Restaurant with id={RestaurantId} not found", tableDto.RestaurantId);
                return new Response<GetTableDto>(HttpStatusCode.BadRequest, "Restaurant not found");
            }
        }

        table.RestaurantId = tableDto.RestaurantId;
        table.Seats = tableDto.Seats;
        table.Status = tableDto.Status;

        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetTableDto>(HttpStatusCode.BadRequest, "Table not updated");

        var dto = mapper.Map<GetTableDto>(table);
        return Response<GetTableDto>.Success(dto);
    }

    public async Task<Response<string>> DeleteTableAsync(int id)
    {
        logger.LogInformation("DeleteTableAsync called with id={Id}", id);

        var table = await context.Tables.FirstOrDefaultAsync(t => t.Id == id);
        if (table == null)
        {
            logger.LogWarning("Table with id={Id} not found", id);
            return new Response<string>(HttpStatusCode.NotFound, "Table not found");
        }

        context.Tables.Remove(table);
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Table not deleted");

        return Response<string>.Success("Table deleted successfully");
    }

    public async Task<Response<bool>> IsTableAvailableAsync(int tableId, DateTime bookingStart, TimeSpan duration)
    {
        logger.LogInformation("IsTableAvailableAsync called with tableId={TableId}, bookingStart={BookingStart}, duration={Duration}", tableId, bookingStart, duration);

        var requestedStart = bookingStart;
        var requestedEnd = requestedStart.Add(duration);

        var overlapExists = await context.Bookings.AnyAsync(b =>
            b.TableId == tableId &&
            b.Status != BookingStatus.Cancelled &&
            (
                b.BookingFrom < requestedEnd &&
                b.BookingTo > requestedStart
            )
        );

        return Response<bool>.Success(!overlapExists);
    }

    public async Task<Response<List<GetTableDto>>> GetAvailableTablesAsync(int restaurantId, DateTime? dateTime, TimeSpan? duration = null)
    {
        logger.LogInformation("GetAvailableTablesAsync called with restaurantId={RestaurantId}, dateTime={DateTime}, duration={Duration}", restaurantId, dateTime, duration);

        var tables = await context.Tables
            .Where(t => t.RestaurantId == restaurantId)
            .AsNoTracking()
            .ToListAsync();

        if (dateTime.HasValue)
        {
            var checkDuration = duration ?? TimeSpan.FromHours(DefaultBookingDurationHours);
            var requestedStart = dateTime.Value;
            var requestedEnd = requestedStart.Add(checkDuration);

            var bookedTableIds = await context.Bookings
                .Where(b => b.Table.RestaurantId == restaurantId
                            && b.Status != BookingStatus.Cancelled
                            && (
                                b.BookingFrom < requestedEnd &&
                                b.BookingTo > requestedStart
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
