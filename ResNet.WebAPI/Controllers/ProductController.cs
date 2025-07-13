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
public class ProductController(IProductService productService)
{
    [HttpGet]
    public async Task<PagedResponse<List<GetProductDto>>> GetAllProducts([FromQuery] ProductFilter filter)
    {
        return await productService.GetAllProductsAsync(filter);
    }

    [HttpGet("{id:int}")]
    public async Task<Response<GetProductDto>> GetProductById(int id)
    {
        return await productService.GetProductByIdAsync(id);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetProductDto>> AddProduct(CreateProductDto productDto)
    {
        return await productService.AddProductAsync(productDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetProductDto>> UpdateProduct(int id, UpdateProductDto productDto)
    {
        return await productService.UpdateProductAsync(id, productDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> DeleteProduct(int id)
    {
        return await productService.DeleteProductAsync(id);
    }

    [HttpPost("{id:int}/photo")]
    [Authorize(Roles = Roles.Admin)]
    [RequestSizeLimit(10_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<Response<string>> UploadProductPhoto([FromRoute] int id, [FromForm] UploadImageDto dto)
    {
        if (dto?.Image == null || dto.Image.Length == 0)
            return new Response<string>(System.Net.HttpStatusCode.BadRequest, "File is required");

        return await productService.UploadProductImageAsync(id, dto.Image);
    }

    [HttpDelete("{id:int}/photo")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> DeleteProductPhoto([FromRoute] int id)
    {
        return await productService.DeleteProductImageAsync(id);
    }
}
