using Domain.Responses;
using ResNet.Domain.Filters;

public interface IRestaurantRequestService
{
    Task<PagedResponse<List<GetRestaurantRequestDto>>> GetAllRequestsAsync(RequestFilter filter);
    Task<Response<GetRestaurantRequestDto>> GetRequestByIdAsync(int id);
    Task<Response<GetRestaurantRequestDto>> CreateRequestAsync(CreateRestaurantRequestDto requestDto);
    Task<Response<GetRestaurantRequestDto>> UpdateRequestStatusAsync(int id, string status);
    Task<Response<string>> DeleteRequestAsync(int id);
}