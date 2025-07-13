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

    [HttpPost("{id:int}/photo")]
    [Authorize(Roles = Roles.Admin)]
    [RequestSizeLimit(10_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<Response<string>> UploadCategoryPhoto([FromRoute] int id, [FromForm] UploadImageDto dto)
    {
        if (dto?.Image == null || dto.Image.Length == 0)
            return new Response<string>(System.Net.HttpStatusCode.BadRequest, "File is required");

        return await categoryService.UploadCategoryImageAsync(id, dto.Image);
    }

    [HttpDelete("{id:int}/photo")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> DeleteCategoryPhoto([FromRoute] int id)
    {
        return await categoryService.DeleteCategoryImageAsync(id);
    }
}
