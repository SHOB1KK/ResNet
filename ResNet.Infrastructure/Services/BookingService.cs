using System.Net;
using System.Linq;
using AutoMapper;
using Domain.Responses;
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
    private string GenerateBookingCode()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private async Task<string> GenerateUniqueBookingCodeAsync()
    {
        string code;
        do
        {
            code = GenerateBookingCode();
        } while (await context.Bookings.AnyAsync(b => b.BookingCode == code));
        return code;
    }

    public async Task<PagedResponse<List<GetBookingDto>>> GetBookingsAsync(BookingFilter filter)
    {
        logger.LogInformation("GetBookingsAsync called with filter {@Filter}", filter);

        IQueryable<Booking> query = context.Bookings
            .Include(b => b.Table)
            .AsNoTracking();

        if (filter.TableId.HasValue)
            query = query.Where(b => b.TableId == filter.TableId);

        if (filter.BookingFrom.HasValue && filter.BookingTo.HasValue)
        {
            query = query.Where(b =>
                b.BookingFrom < filter.BookingTo.Value &&
                b.BookingTo > filter.BookingFrom.Value
            );
        }
        else if (filter.BookingFrom.HasValue)
        {
            var dayStart = filter.BookingFrom.Value.Date;
            var dayEnd = dayStart.AddDays(1);
            query = query.Where(b => b.BookingFrom < dayEnd && b.BookingTo > dayStart);
        }

        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);
        var totalCount = await query.CountAsync();

        var bookings = await query
            .OrderByDescending(b => b.BookingFrom)
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

        if (string.IsNullOrWhiteSpace(bookingDto.FullName) || string.IsNullOrWhiteSpace(bookingDto.PhoneNumber))
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "FullName and PhoneNumber are required for booking");

        if (bookingDto.BookingFrom <= DateTime.UtcNow)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking start time must be in the future");

        if (bookingDto.BookingTo <= bookingDto.BookingFrom)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking end time must be after the start time");

        var table = await context.Tables.FindAsync(bookingDto.TableId);
        if (table == null)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Table not found");

        if (bookingDto.Guests < 1 || bookingDto.Guests > table.Seats)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, $"Guests number must be between 1 and {table.Seats}");

        var overlapExists = await context.Bookings.AnyAsync(b =>
            b.TableId == bookingDto.TableId &&
            b.Status != BookingStatus.Cancelled &&
            bookingDto.BookingFrom < b.BookingTo &&
            bookingDto.BookingTo > b.BookingFrom
        );

        if (overlapExists)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Table is already booked for the selected time range");

        var booking = mapper.Map<Booking>(bookingDto);
        booking.Status = BookingStatus.Pending;
        booking.BookingCode = await GenerateUniqueBookingCodeAsync();

        await context.Bookings.AddAsync(booking);
        var saved = await context.SaveChangesAsync();

        if (saved == 0)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking creation failed");

        var dto = mapper.Map<GetBookingDto>(booking);

        return Response<GetBookingDto>.Success(dto, $"Booking created successfully. Your Booking Code: {booking.BookingCode}");
    }

    public async Task<Response<GetBookingDto>> UpdateBookingAsync(int id, UpdateBookingDto bookingDto)
    {
        logger.LogInformation("UpdateBookingAsync called with id={Id}", id);

        var booking = await context.Bookings.FindAsync(id);
        if (booking == null)
            return new Response<GetBookingDto>(HttpStatusCode.NotFound, "Booking not found");

        if (string.IsNullOrWhiteSpace(bookingDto.FullName) || string.IsNullOrWhiteSpace(bookingDto.PhoneNumber))
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "FullName and PhoneNumber are required for booking");

        if (bookingDto.BookingFrom <= DateTime.UtcNow)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking start time must be in the future");

        if (bookingDto.BookingTo <= bookingDto.BookingFrom)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking end time must be after the start time");

        var table = await context.Tables.FindAsync(booking.TableId);
        if (table == null)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Table not found");

        if (bookingDto.Guests < 1 || bookingDto.Guests > table.Seats)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, $"Guests number must be between 1 and {table.Seats}");

        var overlapExists = await context.Bookings.AnyAsync(b =>
            b.Id != id &&
            b.TableId == booking.TableId &&
            b.Status != BookingStatus.Cancelled &&
            bookingDto.BookingFrom < b.BookingTo &&
            bookingDto.BookingTo > b.BookingFrom
        );

        if (overlapExists)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Table is already booked for the selected time range");

        booking.FullName = bookingDto.FullName;
        booking.PhoneNumber = bookingDto.PhoneNumber;
        booking.Guests = bookingDto.Guests;
        booking.Status = bookingDto.Status;
        booking.BookingFrom = bookingDto.BookingFrom;
        booking.BookingTo = bookingDto.BookingTo;

        var saved = await context.SaveChangesAsync();

        if (saved == 0)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking update failed");

        var dto = mapper.Map<GetBookingDto>(booking);
        return Response<GetBookingDto>.Success(dto);
    }

    public async Task<Response<GetBookingDto>> UpdateBookingByCodeAsync(string bookingCode, string phoneNumber, UpdateBookingDto dto)
    {
        logger.LogInformation("UpdateBookingByCodeAsync called for code={BookingCode}, phone={PhoneNumber}", bookingCode, phoneNumber);

        var booking = await context.Bookings
            .Include(b => b.Table)
            .FirstOrDefaultAsync(b => b.BookingCode == bookingCode && b.PhoneNumber == phoneNumber);

        if (booking == null)
            return new Response<GetBookingDto>(HttpStatusCode.NotFound, "Booking not found with provided code and phone number");

        if (dto.BookingFrom <= DateTime.UtcNow)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking start time must be in the future");

        if (dto.BookingTo <= dto.BookingFrom)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Booking end time must be after the start time");

        if (dto.Guests < 1 || dto.Guests > booking.Table.Seats)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, $"Guests number must be between 1 and {booking.Table.Seats}");

        var overlapExists = await context.Bookings.AnyAsync(b =>
            b.Id != booking.Id &&
            b.TableId == booking.TableId &&
            b.Status != BookingStatus.Cancelled &&
            dto.BookingFrom < b.BookingTo &&
            dto.BookingTo > b.BookingFrom
        );

        if (overlapExists)
            return new Response<GetBookingDto>(HttpStatusCode.BadRequest, "Table is already booked for the selected time range");

        booking.FullName = dto.FullName ?? booking.FullName;
        booking.PhoneNumber = dto.PhoneNumber ?? booking.PhoneNumber;
        booking.Guests = dto.Guests;
        booking.Status = dto.Status;
        booking.BookingFrom = dto.BookingFrom;
        booking.BookingTo = dto.BookingTo;

        await context.SaveChangesAsync();

        var responseDto = mapper.Map<GetBookingDto>(booking);
        return Response<GetBookingDto>.Success(responseDto, "Booking updated successfully");
    }

    public async Task<Response<string>> CancelBookingByCodeAsync(string bookingCode, string phoneNumber)
    {
        logger.LogInformation("CancelBookingByCodeAsync called with code={BookingCode}, phone={PhoneNumber}", bookingCode, phoneNumber);

        var booking = await context.Bookings
            .FirstOrDefaultAsync(b => b.BookingCode == bookingCode && b.PhoneNumber == phoneNumber);

        if (booking == null)
            return new Response<string>(HttpStatusCode.NotFound, "Booking not found with provided code and phone number");

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
}
