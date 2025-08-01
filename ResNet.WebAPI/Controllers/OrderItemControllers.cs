using Domain.Constants;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Constants;
using ResNet.Domain.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class OrderItemController(IOrderItemService orderItemService)
{
    [HttpGet("by-order/{orderId:int}")]
    public async Task<PagedResponse<List<GetOrderItemDto>>> GetByOrderId(int orderId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return await orderItemService.GetOrderItemsByOrderIdAsync(orderId, pageNumber, pageSize);
    }

    [HttpGet]
    public async Task<PagedResponse<List<GetOrderItemDto>>> GetAll([FromQuery] OrderItemFilter filter)
    {
        return await orderItemService.GetOrderItemsAsync(filter);
    }

    [HttpGet("{id:int}")]
    public async Task<Response<GetOrderItemDto>> GetById(int id)
    {
        return await orderItemService.GetOrderItemByIdAsync(id);
    }

    // [HttpPost]
    // public async Task<Response<GetOrderItemDto>> Create(CreateOrderItemDto dto)
    // {
    //     return await orderItemService.AddOrderItemAsync(dto);
    // }

    [HttpPut("{id:int}")]
    public async Task<Response<GetOrderItemDto>> Update(int id, UpdateOrderItemDto dto)
    {
        return await orderItemService.UpdateOrderItemAsync(id, dto);
    }

    [HttpDelete("{id:int}")]
    public async Task<Response<string>> Delete(int id)
    {
        return await orderItemService.DeleteOrderItemAsync(id);
    }
}
