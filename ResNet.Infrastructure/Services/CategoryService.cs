using System.Net;
using AutoMapper;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;
using ResNet.Domain.Filters;

namespace Infrastructure.Services;

public class CategoryService(
    DataContext context,
    IMapper mapper,
    IFileService fileService,
    ILogger<CategoryService> logger
) : ICategoryService
{
    public async Task<PagedResponse<List<GetCategoryDto>>> GetAllCategoriesAsync(CategoryFilter filter)
    {
        logger.LogInformation("GetAllCategoriesAsync called with filter {@Filter}", filter);

        IQueryable<Category> query = context.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var nameLower = filter.Name.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(nameLower));
        }

        int totalCount = await query.CountAsync();

        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        var categories = await query
            .OrderBy(c => c.Name)
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetCategoryDto>>(categories);

        return new PagedResponse<List<GetCategoryDto>>(dtos, validFilter.PageNumber, validFilter.PageSize, totalCount);
    }

    public async Task<Response<GetCategoryDto>> GetCategoryByIdAsync(int id)
    {
        logger.LogInformation("GetCategoryByIdAsync called with id={Id}", id);

        var category = await context.Categories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            logger.LogWarning("Category with id={Id} not found", id);
            return new Response<GetCategoryDto>(HttpStatusCode.NotFound, "Category not found");
        }

        var dto = mapper.Map<GetCategoryDto>(category);
        return Response<GetCategoryDto>.Success(dto);
    }

    public async Task<Response<GetCategoryDto>> AddCategoryAsync(CreateCategoryDto categoryDto)
    {
        logger.LogInformation("AddCategoryAsync called with Name={Name}", categoryDto.Name);

        var exists = await context.Categories
            .AnyAsync(c => c.Name.ToLower() == categoryDto.Name.Trim().ToLower());

        if (exists)
        {
            return new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Category name is already taken");
        }

        var category = mapper.Map<Category>(categoryDto);

        await context.Categories.AddAsync(category);
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Category not created");

        var dto = mapper.Map<GetCategoryDto>(category);
        return Response<GetCategoryDto>.Success(dto);
    }

    public async Task<Response<GetCategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
    {
        logger.LogInformation("UpdateCategoryAsync called with id={Id}", id);

        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            logger.LogWarning("Category with id={Id} not found", id);
            return new Response<GetCategoryDto>(HttpStatusCode.NotFound, "Category not found");
        }

        if (!string.Equals(category.Name, categoryDto.Name, StringComparison.OrdinalIgnoreCase))
        {
            var nameTaken = await context.Categories
                .AnyAsync(c => c.Name.ToLower() == categoryDto.Name.Trim().ToLower() && c.Id != id);

            if (nameTaken)
            {
                return new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Category name is already taken");
            }
        }

        category.Name = categoryDto.Name.Trim();
        category.Description = categoryDto.Description;
        category.ImageUrl = categoryDto.ImageUrl;

        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Category not updated");

        var dto = mapper.Map<GetCategoryDto>(category);
        return Response<GetCategoryDto>.Success(dto);
    }

    public async Task<Response<string>> DeleteCategoryAsync(int id)
    {
        logger.LogInformation("DeleteCategoryAsync called with id={Id}", id);

        var category = await context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            logger.LogWarning("Category with id={Id} not found", id);
            return new Response<string>(HttpStatusCode.NotFound, "Category not found");
        }

        if (category.Products.Any())
        {
            logger.LogWarning("Cannot delete category id={Id} because it has related products", id);
            return new Response<string>(HttpStatusCode.BadRequest, "Category cannot be deleted because it has related products");
        }

        context.Categories.Remove(category);

        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Category not deleted");

        return Response<string>.Success("Category deleted successfully");
    }

    public async Task<Response<string>> UploadCategoryImageAsync(int categoryId, IFormFile file)
    {
        logger.LogInformation("UploadCategoryImageAsync called with categoryId={Id}", categoryId);

        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        if (category == null)
            return new Response<string>(HttpStatusCode.NotFound, "Category not found");

        if (file == null || file.Length == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "File is empty");

        var imageUrl = await fileService.UploadFileAsync(file, "categories");

        if (!string.IsNullOrWhiteSpace(category.ImageUrl))
            await fileService.DeleteFileAsync(category.ImageUrl);

        category.ImageUrl = imageUrl;

        var result = await context.SaveChangesAsync();
        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to update category image");

        return Response<string>.Success(imageUrl);
    }

    public async Task<Response<string>> DeleteCategoryImageAsync(int categoryId)
    {
        logger.LogInformation("DeleteCategoryImageAsync called with categoryId={Id}", categoryId);

        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        if (category == null)
            return new Response<string>(HttpStatusCode.NotFound, "Category not found");

        if (string.IsNullOrWhiteSpace(category.ImageUrl))
            return new Response<string>(HttpStatusCode.BadRequest, "No image to delete");

        var deleted = await fileService.DeleteFileAsync(category.ImageUrl);
        if (!deleted)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to delete image file");

        category.ImageUrl = null;

        var result = await context.SaveChangesAsync();
        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to update category after deleting image");

        return Response<string>.Success("Category image deleted successfully");
    }
}
