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
    public async Task<Response<GetBookingDto>> UpdateBooking(int id, UpdateBookingDto bookingDto)
    {
        return await bookingService.UpdateBookingAsync(id, bookingDto);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<Response<string>> CancelBooking(int id)
    {
        return await bookingService.CancelBookingAsync(id);
    }

    [HttpDelete("{id:int}")]
    public async Task<Response<string>> DeleteBooking(int id)
    {
        return await bookingService.DeleteBookingAsync(id);
    }
}
