using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.Interfaces.Factories;

public interface IOrderItemFactory : IBaseFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="orderId"></param>
    /// <param name="orderAdditions"></param>
    /// <param name="isFree"></param>
    /// <returns></returns>
    OrderItem CreateFromDto(OrderItemDto dto, Guid orderId, List<IAddition> orderAdditions, bool isFree = false);
    OrderItem CreateFromDto(OrderItem entity, OrderItemDto dto, Product product, OrderItemStatuses? status = null, List<IAddition>? orderAdditions = null, bool isFree = false);
    OrderItem CreateFromDto(OrderItemStatuses nextOrderItemStatus, OrderItem entity, OrderStatuses nextOrderStatus);
    OrderItemDto CreateFromEntity(OrderItem entity);
}
