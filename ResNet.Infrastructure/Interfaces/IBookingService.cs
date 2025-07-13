using Domain.Responses;
using ResNet.Domain.Dtos;
using ResNet.Domain.Filters;

namespace Infrastructure.Interfaces;

public interface IBookingService
{
    Task<Response<GetBookingDto>> CreateBookingAsync(CreateBookingDto dto);
    Task<Response<GetBookingDto>> UpdateBookingAsync(int id, UpdateBookingDto dto);
    Task<Response<string>> DeleteBookingAsync(int id);
    Task<Response<string>> CancelBookingAsync(int id);
    Task<Response<GetBookingDto>> GetBookingByIdAsync(int id);
    Task<PagedResponse<List<GetBookingDto>>> GetBookingsAsync(BookingFilter filter);
}
