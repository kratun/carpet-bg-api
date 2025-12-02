using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Factories;

public interface IOrderItemFactory
{
    OrderItem CreateFromDto(OrderItemDto dto, Guid orderId);
    OrderItemDto CreateFromEntity(OrderItem entity);
}
