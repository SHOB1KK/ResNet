using Domain.Constants;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Constants;
using ResNet.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantController(IRestaurantService restaurantService)
{
    [HttpGet]
    public async Task<PagedResponse<List<GetRestaurantDto>>> GetAll([FromQuery] RestaurantFilter filter)
    {
        return await restaurantService.GetAllRestaurantsAsync(filter);
    }

    [HttpGet("{id:int}")]
    public async Task<Response<GetRestaurantDto>> GetById(int id)
    {
        return await restaurantService.GetRestaurantByIdAsync(id);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetRestaurantDto>> Create(CreateRestaurantDto dto)
    {
        return await restaurantService.AddRestaurantAsync(dto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetRestaurantDto>> Update(int id, UpdateRestaurantDto dto)
    {
        return await restaurantService.UpdateRestaurantAsync(id, dto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> Delete(int id)
    {
        return await restaurantService.DeleteRestaurantAsync(id);
    }

    [HttpGet("{id:int}/menu")]
    public async Task<Response<List<GetProductDto>>> GetMenu(int id)
    {
        return await restaurantService.GetMenuByRestaurantIdAsync(id);
    }

    [HttpGet("{id:int}/available-tables")]
    public async Task<Response<List<GetTableDto>>> GetAvailableTables(int id, [FromQuery] DateTime? dateTime)
    {
        return await restaurantService.GetAvailableTablesAsync(id, dateTime);
    }
}
