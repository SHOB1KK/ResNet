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
}
