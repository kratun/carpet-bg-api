using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Factories;

public interface IOrderItemFactory
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
    OrderItemDto CreateFromEntity(OrderItem entity);
}
