using System.Net;
using AutoMapper;
using Domain.Responses;
using Domain.Constants;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;
using ResNet.Domain.Constants;
using ResNet.Domain.Filters;

namespace Infrastructure.Services;

public class BookingService(
    DataContext context,
    IMapper mapper,
    ILogger<BookingService> logger
) : IBookingService
{
    private const int BookingDurationHours = 2;

    public async Task<PagedResponse<List<GetBookingDto>>> GetBookingsAsync(BookingFilter filter)
    {
        logger.LogInformation("GetBookingsAsync called with filter {@Filter}", filter);

        IQueryable<Booking> query = context.Bookings
            .Include(b => b.Table)
            .Include(b => b.User)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(filter.UserId))
            query = query.Where(b => b.UserId == filter.UserId);

        if (filter.TableId.HasValue)
            query = query.Where(b => b.TableId == filter.TableId);

        if (filter.Date.HasValue)
        {
            var dayStart = filter.Date.Value.Date;
            var dayEnd = dayStart.AddDays(1);
            query = query.Where(b => b.BookingTime >= dayStart && b.BookingTime < dayEnd);
        }

        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);
        var totalCount = await query.CountAsync();

        var bookings = await query
            .OrderByDescending(b => b.BookingTime)
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetBookingDto>>(bookings);

        return new PagedResponse<List<GetBookingDto>>(dtos, validFilter.PageNumber, validFilter.PageSize, totalCount);
    }

    public async Task<Response<GetBookingDto>> GetBookingByIdAsync(int id)
    {
        logger.LogInformation("GetBookingByIdAsync called with id={Id}", id);

        var booking = await context.Bookings
            .Include(b => b.Table)
            .Include(b => b.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            logger.LogWarning("Booking with id={Id} not found", id);
            return new Response<GetBookingDto>(HttpStatusCode.NotFound, "Booking not found");
        }

        var dto = mapper.Map<GetBookingDto>(booking);
        return Response<GetBookingDto>.Success(dto);
    }

    public async Task<Response<GetBookingDto>> CreateBookingAsync(CreateBookingDto bookingDto)
    {
        logger.LogInformation("CreateBookingAsync called");

        if (bookingDto.BookingTime <= DateTime.UtcNow)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking time must be in the future");

        var table = await context.Tables.FindAsync(bookingDto.TableId);
        if (table == null)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Table not found");

        if (bookingDto.Guests < 1 || bookingDto.Guests > table.Seats)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, $"Guests number must be between 1 and {table.Seats}");

        var overlapExists = await context.Bookings.AnyAsync(b =>
            b.TableId == bookingDto.TableId &&
            b.Status != BookingStatus.Cancelled &&
            (
                bookingDto.BookingTime < b.BookingTime.AddHours(BookingDurationHours) &&
                bookingDto.BookingTime.AddHours(BookingDurationHours) > b.BookingTime
            )
        );

        if (overlapExists)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Table is already booked for the selected time");

        var booking = mapper.Map<Booking>(bookingDto);
        booking.Status = BookingStatus.Pending;

        await context.Bookings.AddAsync(booking);
        var saved = await context.SaveChangesAsync();

        if (saved == 0)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking creation failed");

        var dto = mapper.Map<GetBookingDto>(booking);
        return Response<GetBookingDto>.Success(dto);
    }

    public async Task<Response<GetBookingDto>> UpdateBookingAsync(int id, UpdateBookingDto bookingDto)
    {
        logger.LogInformation("UpdateBookingAsync called with id={Id}", id);

        var booking = await context.Bookings.FindAsync(id);
        if (booking == null)
            return new Response<GetBookingDto>(HttpStatusCode.NotFound, "Booking not found");

        if (bookingDto.BookingTime <= DateTime.UtcNow)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking time must be in the future");

        var table = await context.Tables.FindAsync(booking.TableId);
        if (table == null)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Table not found");

        if (bookingDto.Guests < 1 || bookingDto.Guests > table.Seats)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, $"Guests number must be between 1 and {table.Seats}");

        var overlapExists = await context.Bookings.AnyAsync(b =>
            b.Id != id &&
            b.TableId == booking.TableId &&
            b.Status != BookingStatus.Cancelled &&
            (
                bookingDto.BookingTime < b.BookingTime.AddHours(BookingDurationHours) &&
                bookingDto.BookingTime.AddHours(BookingDurationHours) > b.BookingTime
            )
        );

        if (overlapExists)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Table is already booked for the selected time");

        booking.BookingTime = bookingDto.BookingTime;
        booking.Guests = bookingDto.Guests;
        booking.Status = bookingDto.Status;

        var saved = await context.SaveChangesAsync();

        if (saved == 0)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking update failed");

        var dto = mapper.Map<GetBookingDto>(booking);
        return Response<GetBookingDto>.Success(dto);
    }

    public async Task<Response<string>> CancelBookingAsync(int id)
    {
        logger.LogInformation("CancelBookingAsync called with id={Id}", id);

        var booking = await context.Bookings.FindAsync(id);
        if (booking == null)
            return new Response<string>(HttpStatusCode.NotFound, "Booking not found");

        if (booking.Status == BookingStatus.Cancelled)
            return new Response<string>(HttpStatusCode.BadRequest, "Booking is already cancelled");

        booking.Status = BookingStatus.Cancelled;
        var saved = await context.SaveChangesAsync();

        if (saved == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Booking cancellation failed");

        return Response<string>.Success("Booking cancelled successfully");
    }

    public async Task<Response<string>> DeleteBookingAsync(int id)
    {
        logger.LogInformation("DeleteBookingAsync called with id={Id}", id);

        var booking = await context.Bookings.FindAsync(id);
        if (booking == null)
            return new Response<string>(HttpStatusCode.NotFound, "Booking not found");

        context.Bookings.Remove(booking);
        var saved = await context.SaveChangesAsync();

        if (saved == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Booking deletion failed");

        return Response<string>.Success("Booking deleted successfully");
    }

    // public async Task<Response<List<GetTableDto>>> GetAvailableTablesAsync(DateTime bookingTime, int guests)
    // {
    //     logger.LogInformation("GetAvailableTablesAsync called for bookingTime={BookingTime}, guests={Guests}", bookingTime, guests);

    //     var tables = await context.Tables
    //         .Where(t => t.Seats >= guests)
    //         .ToListAsync();

    //     var availableTables = new List<Table>();

    //     foreach (var table in tables)
    //     {
    //         bool isBooked = await context.Bookings.AnyAsync(b =>
    //             b.TableId == table.Id &&
    //             b.Status != BookingStatus.Cancelled &&
    //             bookingTime < b.BookingTime.AddHours(BookingDurationHours) &&
    //             bookingTime.AddHours(BookingDurationHours) > b.BookingTime
    //         );

    //         if (!isBooked)
    //             availableTables.Add(table);
    //     }

    //     var dtos = mapper.Map<List<GetTableDto>>(availableTables);
    //     return Response<List<GetTableDto>>.Success(dtos);
    // }

    // public async Task<PagedResponse<List<GetBookingDto>>> GetUserBookingsAsync(string userId, int pageNumber, int pageSize)
    // {
    //     logger.LogInformation("GetUserBookingsAsync called for userId={UserId}", userId);

    //     IQueryable<Booking> query = context.Bookings
    //         .Where(b => b.UserId == userId)
    //         .Include(b => b.Table)
    //         .Include(b => b.User)
    //         .AsNoTracking();

    //     var validFilter = new ValidFilter(pageNumber, pageSize);
    //     var totalCount = await query.CountAsync();

    //     var bookings = await query
    //         .OrderByDescending(b => b.BookingTime)
    //         .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
    //         .Take(validFilter.PageSize)
    //         .ToListAsync();

    //     var dtos = mapper.Map<List<GetBookingDto>>(bookings);

    //     return new PagedResponse<List<GetBookingDto>>(dtos, validFilter.PageNumber, validFilter.PageSize, totalCount);
    // }
}
