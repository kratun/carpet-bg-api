using CarpetBG.Application.DTOs.Additions;
using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.Factories;

public class OrderItemFactory : BaseFactory, IOrderItemFactory
{
    public OrderItem CreateFromDto(OrderItemDto dto, Guid orderId, List<IAddition> orderAdditions, bool isFree = false)
    {
        return new()
        {
            Id = dto.Id ?? Guid.NewGuid(),
            Height = dto.Height,
            Width = dto.Width,
            Price = dto.Price,
            Note = dto.Note,
            OrderId = orderId,
            ProductId = dto.ProductId,
            Status = OrderItemStatuses.New,
            Additions = orderAdditions.Select(a => new Addition
            {
                AdditionType = a.AdditionType,
                Id = Guid.NewGuid(),
                Name = a.Name,
                NormalizedName = a.NormalizedName,
                Value = a.Value
            })
            .ToList() ?? [],
        };
    }

    public OrderItemDto CreateFromEntity(OrderItem entity)
    {
        return new()
        {
            Id = entity.Id,
            Height = entity.Height,
            Width = entity.Width,
            Price = entity.Price,
            Note = entity.Note,
            Additions = [.. entity.Additions.Select(a => new AdditionDto { // TODO Move it to factory
                AdditionType = a.AdditionType,
                Name = a.Name,
                NormalizedName = a.NormalizedName,
                Value = a.Value })],
        };
    }

    public OrderItem CreateFromDto(OrderItemDto dto, OrderItem entity, List<IAddition> orderAdditions, bool isFree = false)
    {
        throw new NotImplementedException();
    }

    public OrderItem CreateFromDto(OrderItemStatuses nextOrderItemStatus, OrderItem entity, OrderStatuses nextOrderStatus)
    {
        entity.Status = nextOrderItemStatus;
        entity.Order.Status = nextOrderStatus;
        return entity;
    }
}
