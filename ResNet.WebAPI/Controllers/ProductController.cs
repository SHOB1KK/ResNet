using Domain.Constants;
using Domain.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Dtos;
using ResNet.Domain.Filters;
using Infrastructure.Interfaces;
using ResNet.Domain.Constants;
using System.Net;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
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
    [Authorize(Roles = Roles.Admin + "," + Roles.Owner + "," + Roles.Cashier)]
    public async Task<Response<GetProductDto>> AddProduct(int restaurantId, CreateProductDto productDto)
    {
        if (!await HasAccessToRestaurant(restaurantId))
            return new Response<GetProductDto>(HttpStatusCode.Forbidden, "Access denied");

        productDto.RestaurantId = restaurantId;
        return await productService.AddProductAsync(productDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Owner + "," + Roles.Cashier)]
    public async Task<Response<GetProductDto>> UpdateProduct(int restaurantId, int id, UpdateProductDto productDto)
    {
        if (!await HasAccessToRestaurant(restaurantId))
            return new Response<GetProductDto>(HttpStatusCode.Forbidden, "Access denied");

        productDto.RestaurantId = restaurantId;
        return await productService.UpdateProductAsync(id, productDto);
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Owner + "," + Roles.Cashier)]
    public async Task<Response<string>> DeleteProduct(int restaurantId, int id)
    {
        if (!await HasAccessToRestaurant(restaurantId))
            return new Response<string>(HttpStatusCode.Forbidden, "Access denied");

        return await productService.DeleteProductAsync(id);
    }

    [HttpPost("{id:int}/photo")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Owner + "," + Roles.Cashier)]
    [RequestSizeLimit(10_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<Response<string>> UploadProductPhoto(int restaurantId, int id, [FromForm] UploadImageDto dto)
    {
        if (!await HasAccessToRestaurant(restaurantId))
            return new Response<string>(HttpStatusCode.Forbidden, "Access denied");

        if (dto?.Image == null || dto.Image.Length == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "File is required");

        return await productService.UploadProductImageAsync(id, dto.Image);
    }

    [HttpDelete("{id:int}/photo")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Owner)]
    public async Task<Response<string>> DeleteProductPhoto(int restaurantId, int id)
    {
        if (!await HasAccessToRestaurant(restaurantId))
            return new Response<string>(HttpStatusCode.Forbidden, "Access denied");

        return await productService.DeleteProductImageAsync(id);
    }

    private async Task<bool> HasAccessToRestaurant(int restaurantId)
    {
        if (User.IsInRole(Roles.Admin))
            return true;

        var claim = User.FindFirst("restaurant_id")?.Value;
        if (string.IsNullOrEmpty(claim))
            return false;

        return int.Parse(claim) == restaurantId;
    }
}
