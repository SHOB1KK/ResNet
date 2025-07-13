using Domain.Responses;
using ResNet.Domain.Dtos;

public interface IOrderService
{
    Task<Response<GetOrderDto>> GetOrderByIdAsync(int id);

    Task<PagedResponse<List<GetOrderDto>>> GetOrdersAsync(OrderFilter filter);

    Task<Response<GetOrderDto>> CreateOrderAsync(CreateOrderDto orderDto);

    Task<Response<GetOrderDto>> CancelOrderAsync(int id);

    Task<Response<GetOrderDto>> UpdateOrderAsync(int id, UpdateOrderDto orderDto);
}
