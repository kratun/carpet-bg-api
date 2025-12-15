using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Shared;

namespace CarpetBG.Application.Interfaces.Services;

public interface IOrderService
{
    Task<Result<PaginatedList<OrderDto>>> GetFilteredAsync(OrderFilterDto filter);
    Task<Result<OrderDto>> GetByIdAsync(Guid id);

    Task<Result<Guid>> CreateOrderAsync(CreateOrderDto dto);
    Task<Result<Guid>> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto dto);
    Task<Result<Guid>> RevertOrderStatusAsync(Guid id, UpdateOrderStatusDto dto);
    Task<Result<Guid>> AddDeliveryDataAsync(Guid id, OrderDeliveryDataDto dto);
    Task<Result<Guid>> ConfirmDeliveryAsync(Guid id, OrderDeliveryConfirmDto dto);
}
