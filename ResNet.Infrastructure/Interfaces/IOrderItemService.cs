using Domain.Responses;
using ResNet.Domain.Dtos;

namespace Infrastructure.Interfaces;

public interface IOrderItemService
{
    Task<PagedResponse<List<GetOrderItemDto>>> GetOrderItemsByOrderIdAsync(int OrderId, int pageNumber, int pageSize);
    Task<PagedResponse<List<GetOrderItemDto>>> GetOrderItemsAsync(OrderItemFilter filter);
    Task<Response<GetOrderItemDto>> GetOrderItemByIdAsync(int id);
    Task<Response<GetOrderItemDto>> AddOrderItemAsync(CreateOrderItemDto orderItemDto);
    Task<Response<GetOrderItemDto>> UpdateOrderItemAsync(int id, UpdateOrderItemDto orderItemDto);
    Task<Response<string>> DeleteOrderItemAsync(int id);
}
