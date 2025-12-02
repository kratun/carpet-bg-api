using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Factories;

public class OrderItemFactory : IOrderItemFactory
{
    public OrderItem CreateFromDto(OrderItemDto dto, Guid orderId)
    {
        return new()
        {
            Id = dto.Id ?? Guid.NewGuid(),
            Diagonal = dto.Diagonal,
            Height = dto.Height,
            Width = dto.Width,
            Price = dto.Price,
            Note = dto.Note,
            OrderId = orderId,
            ProductId = dto.ProductId,
        };
    }

    public OrderItemDto CreateFromEntity(OrderItem entity)
    {
        return new()
        {
            Id = entity.Id,
            Diagonal = entity.Diagonal,
            Height = entity.Height,
            Width = entity.Width,
            Price = entity.Price,
            Note = entity.Note,
            Additions = [.. entity.Additions.Select(a => a.Id)],
        };
    }
}
