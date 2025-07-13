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
public class TableController(ITableService tableService)
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
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetTableDto>> AddTable(int restaurantId, CreateTableDto tableDto)
    {
        tableDto.RestaurantId = restaurantId;
        return await tableService.AddTableAsync(tableDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetTableDto>> UpdateTable(int restaurantId, int id, UpdateTableDto tableDto)
    {
        tableDto.RestaurantId = restaurantId;
        return await tableService.UpdateTableAsync(id, tableDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> DeleteTable(int restaurantId, int id)
    {
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
}
