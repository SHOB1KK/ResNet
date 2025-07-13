using Domain.Responses;
using Microsoft.AspNetCore.Http;
using ResNet.Domain.Dtos;
using ResNet.Domain.Filters;

namespace Infrastructure.Interfaces;

public interface ICategoryService
{
    Task<PagedResponse<List<GetCategoryDto>>> GetAllCategoriesAsync(CategoryFilter filter);
    Task<Response<GetCategoryDto>> GetCategoryByIdAsync(int id);
    Task<Response<GetCategoryDto>> AddCategoryAsync(CreateCategoryDto categoryDto);
    Task<Response<GetCategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
    Task<Response<string>> DeleteCategoryAsync(int id);
    Task<Response<string>> UploadCategoryImageAsync(int categoryId, IFormFile file);
    Task<Response<string>> DeleteCategoryImageAsync(int categoryId);
}