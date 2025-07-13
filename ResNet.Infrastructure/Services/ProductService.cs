using System.Net;
using AutoMapper;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;
using ResNet.Domain.Filters;

namespace Infrastructure.Services;

public class ProductService(
    DataContext context,
    IMapper mapper,
    ILogger<ProductService> logger
) : IProductService
{
    public async Task<PagedResponse<List<GetProductDto>>> GetAllProductsAsync(ProductFilter filter)
    {
        logger.LogInformation("GetAllProductsAsync called");

        IQueryable<Product> query = context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(p => p.Name.ToLower().Contains(filter.Name.ToLower()));

        if (filter.CategoryId != null)
            query = query.Where(p => p.CategoryId == filter.CategoryId);

        if (filter.PriceFrom != null)
            query = query.Where(p => p.Price >= filter.PriceFrom);

        if (filter.PriceTo != null)
            query = query.Where(p => p.Price <= filter.PriceTo);

        int totalCount = await query.CountAsync();

        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        var products = await query
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetProductDto>>(products);

        return new PagedResponse<List<GetProductDto>>(dtos, validFilter.PageNumber, validFilter.PageSize, totalCount);
    }

    public async Task<Response<GetProductDto>> GetProductByIdAsync(int id)
    {
        logger.LogInformation("GetProductByIdAsync called with id={Id}", id);

        var product = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            logger.LogWarning("Product with id={Id} not found", id);
            return new Response<GetProductDto>(HttpStatusCode.NotFound, "Product not found");
        }

        var dto = mapper.Map<GetProductDto>(product);
        return Response<GetProductDto>.Success(dto);
    }

    public async Task<Response<GetProductDto>> AddProductAsync(CreateProductDto productDto)
    {
        logger.LogInformation("AddProductAsync called");

        var product = mapper.Map<Product>(productDto);

        await context.Products.AddAsync(product);
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Product not created");

        var dto = mapper.Map<GetProductDto>(product);
        return Response<GetProductDto>.Success(dto);
    }

    public async Task<Response<GetProductDto>> UpdateProductAsync(int id, UpdateProductDto productDto)
    {
        logger.LogInformation("UpdateProductAsync called with id={Id}", id);

        var existingProduct = await context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (existingProduct == null)
        {
            logger.LogWarning("Product with id={Id} not found", id);
            return new Response<GetProductDto>(HttpStatusCode.NotFound, "Product not found");
        }

        existingProduct.Name = productDto.Name;
        existingProduct.Price = productDto.Price;
        existingProduct.Description = productDto.Description;
        existingProduct.ImageUrl = productDto.ImageUrl;
        existingProduct.Quantity = productDto.Quantity;
        existingProduct.CategoryId = productDto.CategoryId;

        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Product not updated");

        var dto = mapper.Map<GetProductDto>(existingProduct);
        return Response<GetProductDto>.Success(dto);
    }

    public async Task<Response<string>> DeleteProductAsync(int id)
    {
        logger.LogInformation("DeleteProductAsync called with id={Id}", id);

        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            logger.LogWarning("Product with id={Id} not found", id);
            return new Response<string>(HttpStatusCode.NotFound, "Product not found");
        }

        context.Products.Remove(product);
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Product not deleted");

        return Response<string>.Success("Product deleted successfully");
    }
}
