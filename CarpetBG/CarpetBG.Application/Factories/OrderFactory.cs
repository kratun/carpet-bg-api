using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.Factories;

public class OrderFactory(IDateTimeProvider dateTimeProvider, IOrderItemFactory orderItemFactory) : IOrderFactory
{
    public Order CreateFromDto(CreateOrderDto dto)
    {
        var orderId = Guid.NewGuid();
        var percentagePriceAddition = dto.IsExpress ? 0.5m : 0.0m;
        return new()
        {
            Id = orderId,
            PickupDate = dateTimeProvider.ToUtc(dto.PickupDate.Date),
            PickupAddressId = dto.PickupAddressId,
            PickupTimeRange = dto.PickupTimeRange,
            UserId = dto.CustomerId,
            Status = OrderStatuses.PendingPickup,
            Note = dto.Note,
            IsExpress = dto.IsExpress,
            PercentagePriceAddition = percentagePriceAddition,
            Items = [.. dto.OrderItems.Select(oi => orderItemFactory.CreateFromDto(oi, orderId))]
        };
    }

    public OrderDto CreateFromEntity(Order order)
    {
        DateTime? localPickupDate = order.PickupDate.HasValue
                ? dateTimeProvider.FromUtc(order.PickupDate.Value)
                : null;

        return new()
        {
            Id = order.Id,
            CustomerId = order.UserId,
            PhoneNumber = order.User.PhoneNumber,
            PickupAddress = order.PickupAddress.DisplayAddress,
            PickupAddressId = order.PickupAddressId,
            PickupDate = localPickupDate,
            PickupTimeRange = order.PickupTimeRange,
            Status = order.Status,
            UserFullName = order.User.FullName,
        };
    }

    public Order UpdateFromDto(OrderDeliveryDataDto dto, Order entity)
    {
        entity.DeliveryDate = dto.DeliveryDate;
        entity.DeliveryAddressId = dto.DeliveryAddressId;

        entity.DeliveryTimeRange = dto.DeliveryTimeRange;
        entity.Note = dto.Note;

        return entity;
    }
}
