using AutoMapper;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class OrderItemService(
    DataContext context,
    IMapper mapper,
    ILogger<OrderItemService> logger
) : IOrderItemService
{
    public async Task<PagedResponse<List<GetOrderItemDto>>> GetOrderItemsByOrderIdAsync(int orderId, int pageNumber, int pageSize)
    {
        logger.LogInformation("GetOrderItemsByOrderIdAsync called with orderId={OrderId}", orderId);

        var query = context.OrderItems
            .AsNoTracking()
            .Where(i => i.OrderId == orderId);

        int totalCount = await query.CountAsync();

        var validFilter = new ValidFilter(pageNumber, pageSize);

        var items = await query
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetOrderItemDto>>(items);

        return new PagedResponse<List<GetOrderItemDto>>(dtos, validFilter.PageNumber, validFilter.PageSize, totalCount);
    }

    public async Task<PagedResponse<List<GetOrderItemDto>>> GetOrderItemsAsync(OrderItemFilter filter)
    {
        logger.LogInformation("GetOrderItemsAsync called with filter {@Filter}", filter);

        var query = context.OrderItems.AsNoTracking();

        if (filter.OrderId.HasValue)
            query = query.Where(i => i.OrderId == filter.OrderId.Value);

        if (filter.ProductId.HasValue)
            query = query.Where(i => i.ProductId == filter.ProductId.Value);

        if (filter.QuantityFrom.HasValue)
            query = query.Where(i => i.Quantity >= filter.QuantityFrom.Value);

        if (filter.QuantityTo.HasValue)
            query = query.Where(i => i.Quantity <= filter.QuantityTo.Value);

        int pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
        int pageSize = filter.PageSize > 0 ? filter.PageSize : 10;

        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetOrderItemDto>>(items);

        return new PagedResponse<List<GetOrderItemDto>>(dtos, pageNumber, pageSize, totalCount);
    }

    public async Task<Response<GetOrderItemDto>> GetOrderItemByIdAsync(int id)
    {
        logger.LogInformation("GetOrderItemByIdAsync called with id={Id}", id);

        var orderItem = await context.OrderItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        if (orderItem == null)
        {
            logger.LogWarning("OrderItem with id={Id} not found", id);
            return new Response<GetOrderItemDto>(System.Net.HttpStatusCode.NotFound, "Order item not found");
        }

        var dto = mapper.Map<GetOrderItemDto>(orderItem);
        return Response<GetOrderItemDto>.Success(dto);
    }

    public async Task<Response<GetOrderItemDto>> AddOrderItemAsync(CreateOrderItemDto orderItemDto)
    {
        logger.LogInformation("AddOrderItemAsync called");

        var orderItem = mapper.Map<OrderItem>(orderItemDto);

        await context.OrderItems.AddAsync(orderItem);
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetOrderItemDto>(System.Net.HttpStatusCode.BadRequest, "Order item not created");

        var dto = mapper.Map<GetOrderItemDto>(orderItem);
        return Response<GetOrderItemDto>.Success(dto);
    }

    public async Task<Response<GetOrderItemDto>> UpdateOrderItemAsync(int id, UpdateOrderItemDto orderItemDto)
    {
        logger.LogInformation("UpdateOrderItemAsync called with id={Id}", id);

        var orderItem = await context.OrderItems.FirstOrDefaultAsync(i => i.Id == id);
        if (orderItem == null)
        {
            logger.LogWarning("OrderItem with id={Id} not found", id);
            return new Response<GetOrderItemDto>(System.Net.HttpStatusCode.NotFound, "Order item not found");
        }

        orderItem.ProductId = orderItemDto.ProductId;
        orderItem.Quantity = orderItemDto.Quantity;
        orderItem.PriceAtMoment = orderItemDto.PriceAtMoment;

        var result = await context.SaveChangesAsync();
        if (result == 0)
            return new Response<GetOrderItemDto>(System.Net.HttpStatusCode.BadRequest, "Order item not updated");

        var dto = mapper.Map<GetOrderItemDto>(orderItem);
        return Response<GetOrderItemDto>.Success(dto);
    }

    public async Task<Response<string>> DeleteOrderItemAsync(int id)
    {
        logger.LogInformation("DeleteOrderItemAsync called with id={Id}", id);

        var orderItem = await context.OrderItems.FirstOrDefaultAsync(i => i.Id == id);
        if (orderItem == null)
        {
            logger.LogWarning("OrderItem with id={Id} not found", id);
            return new Response<string>(System.Net.HttpStatusCode.NotFound, "Order item not found");
        }

        context.OrderItems.Remove(orderItem);
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<string>(System.Net.HttpStatusCode.BadRequest, "Order item not deleted");

        return Response<string>.Success("Order item deleted successfully");
    }
}
