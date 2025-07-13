using System.Net;
using AutoMapper;
using Domain.Responses;
using Domain.Constants;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResNet.Domain.Dtos;
using ResNet.Domain.Constants;
using ResNet.Domain.Entities;

namespace Infrastructure.Services;

public class OrderService(
    DataContext context,
    IMapper mapper,
    ILogger<OrderService> logger
) : IOrderService
{
    public async Task<Response<GetOrderDto>> GetOrderByIdAsync(int id)
    {
        logger.LogInformation("GetOrderByIdAsync called with id={Id}", id);

        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            logger.LogWarning("Order with id={Id} not found", id);
            return new Response<GetOrderDto>(HttpStatusCode.NotFound, "Order not found");
        }

        var dto = mapper.Map<GetOrderDto>(order);
        return Response<GetOrderDto>.Success(dto);
    }

    public async Task<PagedResponse<List<GetOrderDto>>> GetOrdersAsync(OrderFilter filter)
    {
        logger.LogInformation("GetOrdersAsync called with filter {@Filter}", filter);

        IQueryable<Order> query = context.Orders.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.FullName))
            query = query.Where(o => o.FullName != null && o.FullName.ToLower().Contains(filter.FullName.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.PhoneNumber))
            query = query.Where(o => o.PhoneNumber != null && o.PhoneNumber.Contains(filter.PhoneNumber));

        if (filter.TotalAmountFrom != null)
            query = query.Where(o => o.TotalAmount >= filter.TotalAmountFrom);

        if (filter.TotalAmountTo != null)
            query = query.Where(o => o.TotalAmount <= filter.TotalAmountTo);

        if (filter.CreatedFrom != null)
            query = query.Where(o => o.CreatedAt >= filter.CreatedFrom);

        if (filter.CreatedTo != null)
            query = query.Where(o => o.CreatedAt <= filter.CreatedTo);

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(o => o.Status == filter.Status);

        int totalCount = await query.CountAsync();

        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetOrderDto>>(orders);

        return new PagedResponse<List<GetOrderDto>>(dtos, validFilter.PageNumber, validFilter.PageSize, totalCount);
    }


    public async Task<Response<GetOrderDto>> CreateOrderAsync(CreateOrderDto orderDto)
    {
        logger.LogInformation("CreateOrderAsync called with Type={OrderType}", orderDto.Type);

        if (string.IsNullOrWhiteSpace(orderDto.FullName) || string.IsNullOrWhiteSpace(orderDto.PhoneNumber))
        {
            return new Response<GetOrderDto>(
                HttpStatusCode.BadRequest,
                "FullName and PhoneNumber are required for all orders."
            );
        }

        switch (orderDto.Type)
        {
            case Domain.Constants.OrderType.Pickup:
                break;

            case Domain.Constants.OrderType.Delivery:
                if (string.IsNullOrWhiteSpace(orderDto.DeliveryAddress))
                {
                    return new Response<GetOrderDto>(
                        HttpStatusCode.BadRequest,
                        "DeliveryAddress is required for Delivery orders."
                    );
                }
                break;

            case Domain.Constants.OrderType.AtTable:
                if (orderDto.TableId == null)
                {
                    return new Response<GetOrderDto>(
                        HttpStatusCode.BadRequest,
                        "TableId is required for AtTable orders."
                    );
                }

                var tableExists = await context.Tables
                    .AsNoTracking()
                    .AnyAsync(t => t.Id == orderDto.TableId);

                if (!tableExists)
                {
                    return new Response<GetOrderDto>(
                        HttpStatusCode.BadRequest,
                        $"Table with id={orderDto.TableId} not found."
                    );
                }
                break;

            default:
                return new Response<GetOrderDto>(
                    HttpStatusCode.BadRequest,
                    "Invalid OrderType. Allowed values: Pickup, Delivery, AtTable."
                );
        }

        var order = mapper.Map<Order>(orderDto);
        order.Status = OrderStatus.Pending;
        order.CreatedAt = DateTime.UtcNow;

        await context.Orders.AddAsync(order);
        var result = await context.SaveChangesAsync();

        if (result == 0)
        {
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Order not created.");
        }

        var dto = mapper.Map<GetOrderDto>(order);
        return Response<GetOrderDto>.Success(dto);
    }

    public async Task<Response<GetOrderDto>> CancelOrderAsync(int id)
    {
        logger.LogInformation("CancelOrderAsync called with id={Id}", id);

        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            logger.LogWarning("Order with id={Id} not found", id);
            return new Response<GetOrderDto>(HttpStatusCode.NotFound, "Order not found");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Order is already canceled");
        }

        order.Status = OrderStatus.Cancelled;
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Failed to cancel order");

        var dto = mapper.Map<GetOrderDto>(order);
        return Response<GetOrderDto>.Success(dto);
    }

    public async Task<Response<GetOrderDto>> UpdateOrderAsync(int id, UpdateOrderDto orderDto)
    {
        logger.LogInformation("UpdateOrderAsync called with id={Id}", id);

        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            logger.LogWarning("Order with id={Id} not found", id);
            return new Response<GetOrderDto>(HttpStatusCode.NotFound, "Order not found");
        }

        order.FullName = orderDto.FullName ?? order.FullName;
        order.PhoneNumber = orderDto.PhoneNumber ?? order.PhoneNumber;
        order.Status = orderDto.Status ?? order.Status;
        order.TotalAmount = orderDto.TotalAmount ?? order.TotalAmount;

        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Order not updated");

        var dto = mapper.Map<GetOrderDto>(order);
        return Response<GetOrderDto>.Success(dto);
    }
}
