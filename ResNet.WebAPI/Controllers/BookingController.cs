using Domain.Constants;
using Domain.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Dtos;
using ResNet.Domain.Filters;
using Infrastructure.Interfaces;
using ResNet.Domain.Constants;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController(IBookingService bookingService)
{
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<PagedResponse<List<GetBookingDto>>> GetBookings([FromQuery] BookingFilter filter)
    {
        return await bookingService.GetBookingsAsync(filter);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetBookingDto>> GetBookingById(int id)
    {
        return await bookingService.GetBookingByIdAsync(id);
    }

    [HttpPost]
    public async Task<Response<GetBookingDto>> CreateBooking(CreateBookingDto bookingDto)
    {
        return await bookingService.CreateBookingAsync(bookingDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetBookingDto>> UpdateBooking(int id, UpdateBookingDto bookingDto)
    {
        return await bookingService.UpdateBookingAsync(id, bookingDto);
    }

    [HttpPut("update-by-code")]
    public async Task<Response<GetBookingDto>> UpdateBookingByCode(
    [FromQuery] string bookingCode,
    [FromQuery] string phoneNumber,
    [FromBody] UpdateBookingDto bookingDto)
    {
        return await bookingService.UpdateBookingByCodeAsync(bookingCode, phoneNumber, bookingDto);
    }

    [HttpPost("cancel-by-code")]
    public async Task<Response<string>> CancelBookingByCode(
        [FromQuery] string bookingCode,
        [FromQuery] string phoneNumber)
    {
        return await bookingService.CancelBookingByCodeAsync(bookingCode, phoneNumber);
    }

    [HttpDelete("{id:int}")]
    public async Task<Response<string>> DeleteBooking(int id)
    {
        return await bookingService.DeleteBookingAsync(id);
    }
}
