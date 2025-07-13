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
public class CategoryController(ICategoryService categoryService)
{
    [HttpGet]
    public async Task<PagedResponse<List<GetCategoryDto>>> GetAllCategories([FromQuery] CategoryFilter filter)
    {
        return await categoryService.GetAllCategoriesAsync(filter);
    }

    [HttpGet("{id:int}")]
    public async Task<Response<GetCategoryDto>> GetCategoryById(int id)
    {
        return await categoryService.GetCategoryByIdAsync(id);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetCategoryDto>> AddCategory(CreateCategoryDto categoryDto)
    {
        return await categoryService.AddCategoryAsync(categoryDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetCategoryDto>> UpdateCategory(int id, UpdateCategoryDto categoryDto)
    {
        return await categoryService.UpdateCategoryAsync(id, categoryDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> DeleteCategory(int id)
    {
        return await categoryService.DeleteCategoryAsync(id);
    }
}
