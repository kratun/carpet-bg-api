using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Shared;

namespace CarpetBG.Application.Interfaces.Services;

public interface IOrderItemService
{
    Task<Result<Guid>> AddAsync(OrderItemDto dto, Guid orderId);
}
