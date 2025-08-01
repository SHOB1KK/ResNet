using System.Net;
using Domain.Constants;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Constants;
using ResNet.Domain.Filters;

namespace WebApi.Controllers;

[ApiController]
[Route("api/restaurants/{restaurantId:int}/tables")]
public class TableController(ITableService tableService) : ControllerBase
{
    [HttpGet]
    public async Task<PagedResponse<List<GetTableDto>>> GetTablesByRestaurantId(int restaurantId, [FromQuery] TableFilter filter)
    {
        return await tableService.GetTablesByRestaurantIdAsync(restaurantId, filter);
    }

    [HttpGet("{id:int}")]
    public async Task<Response<GetTableDto>> GetTableById(int restaurantId, int id)
    {
        return await tableService.GetTableByIdAsync(id);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin + "," + Roles.Owner)]
    public async Task<Response<GetTableDto>> AddTable(int restaurantId, CreateTableDto tableDto)
    {
        if (!await HasAccessToRestaurant(restaurantId))
            return new Response<GetTableDto>(HttpStatusCode.Forbidden, "Access denied");

        tableDto.RestaurantId = restaurantId;
        return await tableService.AddTableAsync(tableDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Owner)]
    public async Task<Response<GetTableDto>> UpdateTable(int restaurantId, int id, UpdateTableDto tableDto)
    {
        if (!await HasAccessToRestaurant(restaurantId))
            return new Response<GetTableDto>(HttpStatusCode.Forbidden, "Access denied");

        tableDto.RestaurantId = restaurantId;
        return await tableService.UpdateTableAsync(id, tableDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin + "," + Roles.Owner)]
    public async Task<Response<string>> DeleteTable(int restaurantId, int id)
    {
        if (!await HasAccessToRestaurant(restaurantId))
            return new Response<string>(HttpStatusCode.Forbidden, "Access denied");

        return await tableService.DeleteTableAsync(id);
    }

    [HttpGet("{id:int}/availability")]
    public async Task<Response<bool>> IsTableAvailable(int restaurantId, int id, [FromQuery] DateTime bookingTime, [FromQuery] TimeSpan? duration)
    {
        var checkDuration = duration ?? TimeSpan.FromHours(2);
        return await tableService.IsTableAvailableAsync(id, bookingTime, checkDuration);
    }

    [HttpGet("available")]
    public async Task<Response<List<GetTableDto>>> GetAvailableTables(int restaurantId, [FromQuery] DateTime? dateTime, [FromQuery] TimeSpan? duration)
    {
        return await tableService.GetAvailableTablesAsync(restaurantId, dateTime, duration);
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
