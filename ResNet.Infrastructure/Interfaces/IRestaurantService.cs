using Domain.Responses;
using Microsoft.AspNetCore.Http;
using ResNet.Domain.Dtos;

namespace Infrastructure.Interfaces;

public interface IRestaurantService
{
    Task<PagedResponse<List<GetRestaurantDto>>> GetAllRestaurantsAsync(RestaurantFilter filter);
    Task<Response<GetRestaurantDto>> GetRestaurantByIdAsync(int id);
    Task<Response<GetRestaurantDto>> AddRestaurantAsync(CreateRestaurantDto dto);
    Task<Response<GetRestaurantDto>> UpdateRestaurantAsync(int id, UpdateRestaurantDto dto);
    Task<Response<string>> DeleteRestaurantAsync(int id);
    Task<Response<string>> UploadRestaurantImageAsync(int restaurantId, IFormFile file);
    Task<Response<string>> DeleteRestaurantImageAsync(int restaurantId);
    Task<Response<GetMenuDto>> GetMenuByRestaurantIdAsync(int restaurantId);
    Task<Response<string>> AddCategoryToRestaurantAsync(int restaurantId, int categoryId);
    Task<Response<List<GetTableDto>>> GetAvailableTablesAsync(int restaurantId, DateTime? dateTime);
}
