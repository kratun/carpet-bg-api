using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Factories;

public interface IOrderFactory
{
    Order CreateFromDto(CreateOrderDto dto);
    OrderDto CreateFromEntity(Order order);
    Order UpdateFromDto(OrderDeliveryDataDto dto, Order entity);
}
