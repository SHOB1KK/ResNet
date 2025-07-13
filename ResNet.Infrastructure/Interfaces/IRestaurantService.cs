using Domain.Responses;
using ResNet.Domain.Dtos;

namespace Infrastructure.Interfaces;

public interface IRestaurantService
{
    Task<PagedResponse<List<GetRestaurantDto>>> GetAllRestaurantsAsync(RestaurantFilter filter);
    Task<Response<GetRestaurantDto>> GetRestaurantByIdAsync(int id);
    Task<Response<GetRestaurantDto>> AddRestaurantAsync(CreateRestaurantDto dto);
    Task<Response<GetRestaurantDto>> UpdateRestaurantAsync(int id, UpdateRestaurantDto dto);
    Task<Response<string>> DeleteRestaurantAsync(int id);
    Task<Response<List<GetTableDto>>> GetAvailableTablesAsync(int restaurantId, DateTime? dateTime);
    Task<Response<List<GetProductDto>>> GetMenuByRestaurantIdAsync(int restaurantId);
}
