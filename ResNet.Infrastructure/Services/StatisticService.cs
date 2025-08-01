using System.Net;
using AutoMapper;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;

namespace Infrastructure.Services;

public class StatisticService(
    DataContext context,
    ILogger<StatisticService> logger,
    IMapper mapper
) : IStatisticService
{
    public async Task<Response<OrderStatisticsDto>> GetGlobalOrderStatisticsAsync(DateTime? from, DateTime? to)
    {
        logger.LogInformation("GetGlobalOrderStatisticsAsync called from={From} to={To}", from, to);

        var query = context.Orders.AsNoTracking();

        if (from.HasValue)
            query = query.Where(o => o.CreatedAt >= from.Value);
        if (to.HasValue)
            query = query.Where(o => o.CreatedAt <= to.Value);

        var totalOrders = await query.CountAsync();
        var totalCompleted = await query.CountAsync(o => o.Status == "Completed");
        var totalCancelled = await query.CountAsync(o => o.Status == "Cancelled");
        var totalRevenue = await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        var stats = new OrderStatisticsDto
        {
            TotalOrders = totalOrders,
            CompletedOrders = totalCompleted,
            CancelledOrders = totalCancelled,
            TotalRevenue = totalRevenue
        };

        return Response<OrderStatisticsDto>.Success(stats);
    }

    public async Task<PagedResponse<List<ProductSalesDto>>> GetGlobalTopSellingProductsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
    {
        logger.LogInformation("GetGlobalTopSellingProductsAsync called from={From} to={To}", from, to);

        IQueryable<OrderItem> query = context.OrderItems
            .AsNoTracking()
            .Include(oi => oi.Product)
            .Include(oi => oi.Order);

        if (from.HasValue)
            query = query.Where(oi => oi.Order.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(oi => oi.Order.CreatedAt <= to.Value);

        var grouped = query
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
            .Select(g => new ProductSalesDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                QuantitySold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.PriceAtMoment)
            })
            .OrderByDescending(x => x.QuantitySold);

        var totalCount = await grouped.CountAsync();

        var data = await grouped
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<List<ProductSalesDto>>(data, pageNumber, pageSize, totalCount);
    }


    public async Task<Response<OrderTypeStatisticsDto>> GetGlobalOrderTypeStatisticsAsync(DateTime? from, DateTime? to)
    {
        logger.LogInformation("GetGlobalOrderTypeStatisticsAsync called");

        var query = context.Orders.AsNoTracking();

        if (from.HasValue)
            query = query.Where(o => o.CreatedAt >= from.Value);
        if (to.HasValue)
            query = query.Where(o => o.CreatedAt <= to.Value);

        var pickupCount = await query.CountAsync(o => o.Type == "Pickup");
        var deliveryCount = await query.CountAsync(o => o.Type == "Delivery");
        var atTableCount = await query.CountAsync(o => o.Type == "AtTable");

        var stats = new OrderTypeStatisticsDto
        {
            PickupOrders = pickupCount,
            DeliveryOrders = deliveryCount,
            AtTableOrders = atTableCount
        };

        return Response<OrderTypeStatisticsDto>.Success(stats);
    }

    public async Task<Response<decimal>> GetGlobalAverageCheckAsync(DateTime? from, DateTime? to)
    {
        logger.LogInformation("GetGlobalAverageCheckAsync called");

        var query = context.Orders.AsNoTracking()
            .Where(o => o.Status == "Completed");

        if (from.HasValue)
            query = query.Where(o => o.CreatedAt >= from.Value);
        if (to.HasValue)
            query = query.Where(o => o.CreatedAt <= to.Value);

        var count = await query.CountAsync();
        if (count == 0)
            return Response<decimal>.Success(0);

        var total = await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        var avg = total / count;

        return Response<decimal>.Success(avg);
    }

    public async Task<Response<List<DailyOrdersDto>>> GetGlobalDailyOrdersTrendAsync(DateTime from, DateTime to)
    {
        logger.LogInformation("GetGlobalDailyOrdersTrendAsync called from={From} to={To}", from, to);

        var orders = await context.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new DailyOrdersDto
            {
                Date = g.Key,
                OrdersCount = g.Count(),
                TotalRevenue = g.Sum(x => (decimal?)x.TotalAmount) ?? 0
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return Response<List<DailyOrdersDto>>.Success(orders);
    }





    public async Task<Response<OrderStatisticsDto>> GetOrderStatisticsAsync(int restaurantId, DateTime? from, DateTime? to)
    {
        logger.LogInformation("GetOrderStatisticsAsync called for RestaurantId={RestaurantId} from={From} to={To}", restaurantId, from, to);

        var query = context.Orders.AsNoTracking()
            .Where(o => o.RestaurantId == restaurantId);

        if (from.HasValue)
            query = query.Where(o => o.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(o => o.CreatedAt <= to.Value);

        var totalOrders = await query.CountAsync();
        var totalCompleted = await query.CountAsync(o => o.Status == "Completed");
        var totalCancelled = await query.CountAsync(o => o.Status == "Cancelled");
        var totalRevenue = await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        var stats = new OrderStatisticsDto
        {
            TotalOrders = totalOrders,
            CompletedOrders = totalCompleted,
            CancelledOrders = totalCancelled,
            TotalRevenue = totalRevenue
        };

        return Response<OrderStatisticsDto>.Success(stats);
    }

    public async Task<PagedResponse<List<ProductSalesDto>>> GetTopSellingProductsAsync(int restaurantId, DateTime? from, DateTime? to, int pageNumber, int pageSize)
    {
        logger.LogInformation("GetTopSellingProductsAsync called for RestaurantId={RestaurantId} from={From} to={To}", restaurantId, from, to);

        var query = context.OrderItems.AsNoTracking()
            .Include(oi => oi.Product)
            .Include(oi => oi.Order)
            .Where(oi => oi.Product.RestaurantId == restaurantId);

        if (from.HasValue)
            query = query.Where(oi => oi.Order.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(oi => oi.Order.CreatedAt <= to.Value);

        var grouped = query.GroupBy(oi => new { oi.ProductId, oi.Product.Name })
            .Select(g => new ProductSalesDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                QuantitySold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.PriceAtMoment)
            })
            .OrderByDescending(x => x.QuantitySold);

        var totalCount = await grouped.CountAsync();

        var data = await grouped
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<List<ProductSalesDto>>(data, pageNumber, pageSize, totalCount);
    }

    public async Task<Response<OrderTypeStatisticsDto>> GetOrderTypeStatisticsAsync(int restaurantId, DateTime? from, DateTime? to)
    {
        logger.LogInformation("GetOrderTypeStatisticsAsync called for RestaurantId={RestaurantId}", restaurantId);

        var query = context.Orders.AsNoTracking()
            .Where(o => o.RestaurantId == restaurantId);

        if (from.HasValue)
            query = query.Where(o => o.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(o => o.CreatedAt <= to.Value);

        var pickupCount = await query.CountAsync(o => o.Type == "Pickup");
        var deliveryCount = await query.CountAsync(o => o.Type == "Delivery");
        var atTableCount = await query.CountAsync(o => o.Type == "AtTable");

        var stats = new OrderTypeStatisticsDto
        {
            PickupOrders = pickupCount,
            DeliveryOrders = deliveryCount,
            AtTableOrders = atTableCount
        };

        return Response<OrderTypeStatisticsDto>.Success(stats);
    }

    public async Task<Response<decimal>> GetAverageCheckAsync(int restaurantId, DateTime? from, DateTime? to)
    {
        logger.LogInformation("GetAverageCheckAsync called for RestaurantId={RestaurantId}", restaurantId);

        var query = context.Orders.AsNoTracking()
            .Where(o => o.RestaurantId == restaurantId && o.Status == "Completed");

        if (from.HasValue)
            query = query.Where(o => o.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(o => o.CreatedAt <= to.Value);

        var count = await query.CountAsync();
        if (count == 0)
            return Response<decimal>.Success(0);

        var total = await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        var avg = total / count;

        return Response<decimal>.Success(avg);
    }

    public async Task<Response<List<DailyOrdersDto>>> GetDailyOrdersTrendAsync(int restaurantId, DateTime from, DateTime to)
    {
        logger.LogInformation("GetDailyOrdersTrendAsync called for RestaurantId={RestaurantId} from={From} to={To}", restaurantId, from, to);

        var orders = await context.Orders.AsNoTracking()
            .Where(o => o.RestaurantId == restaurantId && o.CreatedAt >= from && o.CreatedAt <= to)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new DailyOrdersDto
            {
                Date = g.Key,
                OrdersCount = g.Count(),
                TotalRevenue = g.Sum(x => (decimal?)x.TotalAmount) ?? 0
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return Response<List<DailyOrdersDto>>.Success(orders);
    }
}
