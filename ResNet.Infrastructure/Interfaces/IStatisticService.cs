using Domain.Responses;
using ResNet.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces;

public interface IStatisticService
{
    // Локальная статистика — по конкретному ресторану
    Task<Response<OrderStatisticsDto>> GetOrderStatisticsAsync(int restaurantId, DateTime? from = null, DateTime? to = null);

    Task<PagedResponse<List<ProductSalesDto>>> GetTopSellingProductsAsync(int restaurantId, DateTime? from = null, DateTime? to = null, int pageNumber = 1, int pageSize = 10);

    Task<Response<OrderTypeStatisticsDto>> GetOrderTypeStatisticsAsync(int restaurantId, DateTime? from = null, DateTime? to = null);

    Task<Response<decimal>> GetAverageCheckAsync(int restaurantId, DateTime? from = null, DateTime? to = null);

    Task<Response<List<DailyOrdersDto>>> GetDailyOrdersTrendAsync(int restaurantId, DateTime from, DateTime to);

    // Глобальная статистика — по всем ресторанам
    Task<Response<OrderStatisticsDto>> GetGlobalOrderStatisticsAsync(DateTime? from = null, DateTime? to = null);

    Task<PagedResponse<List<ProductSalesDto>>> GetGlobalTopSellingProductsAsync(DateTime? from = null, DateTime? to = null, int pageNumber = 1, int pageSize = 10);

    Task<Response<OrderTypeStatisticsDto>> GetGlobalOrderTypeStatisticsAsync(DateTime? from = null, DateTime? to = null);

    Task<Response<decimal>> GetGlobalAverageCheckAsync(DateTime? from = null, DateTime? to = null);

    Task<Response<List<DailyOrdersDto>>> GetGlobalDailyOrdersTrendAsync(DateTime from, DateTime to);
}
