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
public class OrderController(IOrderService orderService)
{
    [HttpGet("{id:int}")]
    public async Task<Response<GetOrderDto>> GetOrderById(int id)
    {
        return await orderService.GetOrderByIdAsync(id);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<PagedResponse<List<GetOrderDto>>> GetOrders([FromQuery] OrderFilter filter)
    {
        return await orderService.GetOrdersAsync(filter);
    }

    [HttpPost]
    public async Task<Response<GetOrderDto>> CreateOrder(CreateOrderDto orderDto)
    {
        return await orderService.CreateOrderAsync(orderDto);
    }

    [HttpPut("{id:int}")]
    public async Task<Response<GetOrderDto>> UpdateOrder(int id, UpdateOrderDto orderDto)
    {
        return await orderService.UpdateOrderAsync(id, orderDto);
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<Response<GetOrderDto>> CancelOrder(int id)
    {
        return await orderService.CancelOrderAsync(id);
    }
}
