using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Responses;
using ResNet.Domain.Dtos;
using ResNet.Domain.Filters;

namespace Infrastructure.Interfaces;

public interface ITableService
{
    Task<PagedResponse<List<GetTableDto>>> GetTablesByRestaurantIdAsync(int restaurantId, TableFilter filter);
    Task<Response<GetTableDto>> GetTableByIdAsync(int id);
    Task<Response<GetTableDto>> AddTableAsync(CreateTableDto tableDto);
    Task<Response<GetTableDto>> UpdateTableAsync(int id, UpdateTableDto tableDto);
    Task<Response<string>> DeleteTableAsync(int id);

    Task<Response<bool>> IsTableAvailableAsync(int tableId, DateTime bookingTime, TimeSpan duration);
    Task<Response<List<GetTableDto>>> GetAvailableTablesAsync(int restaurantId, DateTime? dateTime, TimeSpan? duration = null);
}
