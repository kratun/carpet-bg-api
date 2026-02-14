using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Factories;

public interface IOrderFactory : IBaseFactory
{
    Order CreateFromDto(CreateOrderDto dto, List<IAddition> orderAdditions);
    OrderDto CreateFromEntity(Order order);
    Order UpdateFromDto(OrderDeliveryDataDto dto, Order entity);
    OrderPrintDto MapToPrintModel(Order order, int minRows = 0);
}
