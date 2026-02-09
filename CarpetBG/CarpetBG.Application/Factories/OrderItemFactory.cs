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

    public OrderItem CreateFromDto(OrderItem entity, OrderItemDto dto, Product product, OrderItemStatuses? status = null, List<IAddition>? orderAdditions = null, bool isFree = false)
    {
        entity.Height = dto.Height;
        entity.Width = dto.Width;
        if (entity.ProductId != dto.ProductId)
        {
            entity.ProductId = dto.ProductId;
            entity.Price = product.Price;
        }

        entity.Note = dto.Note;
        if (status.HasValue)
        {
            entity.Status = status.Value;
        }

        // TODO: add additions

        return entity;
    }

    public OrderItem CreateFromDto(OrderItemStatuses nextOrderItemStatus, OrderItem entity, OrderStatuses nextOrderStatus)
    {
        entity.Status = nextOrderItemStatus;
        entity.Order.Status = nextOrderStatus;
        return entity;
    }
}
