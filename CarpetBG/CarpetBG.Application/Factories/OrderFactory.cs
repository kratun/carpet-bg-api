using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.Factories;

public class OrderFactory(IDateTimeProvider dateTimeProvider, IOrderItemFactory orderItemFactory) : BaseFactory, IOrderFactory
{
    public Order CreateFromDto(CreateOrderDto dto, List<IAddition> orderAdditions)
    {
        var orderId = Guid.NewGuid();

        return new()
        {
            Id = orderId,
            PickupDate = dto.PickupDate?.Date,
            PickupAddressId = dto.PickupAddressId,
            PickupTimeRange = dto.PickupTimeRange,
            CustomerId = dto.CustomerId,
            Status = OrderStatuses.PendingPickup,
            Note = dto.Note ?? string.Empty,// TODO add migration for nullable
            Items = [.. dto.OrderItems.Select(oi => orderItemFactory.CreateFromDto(oi, orderId, orderAdditions))]
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
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            PhoneNumber = order.Customer.PhoneNumber,
            PickupAddress = order.PickupAddress.DisplayAddress,
            PickupAddressId = order.PickupAddressId,
            PickupDate = localPickupDate,
            PickupTimeRange = order.PickupTimeRange,
            Status = order.Status,
            CustomerFullName = order.Customer.FullName,
        };
    }

    public Order UpdateFromDto(OrderDeliveryDataDto dto, Order entity)
    {
        var isPickup = entity.Status == OrderStatuses.PendingPickup || entity.Status == OrderStatuses.New;
        if (isPickup)
        {
            entity.PickupDate = dto.Date;
            entity.PickupAddressId = dto.AddressId;
            entity.Status = OrderStatuses.PendingPickup;
            entity.PickupTimeRange = dto.TimeRange;
        }

        var isDelivery = entity.Status == OrderStatuses.PendingDelivery || entity.Status == OrderStatuses.WashingComplete;
        if (isDelivery)
        {
            entity.DeliveryDate = dto.Date;
            entity.DeliveryAddressId = dto.AddressId;
            entity.Status = OrderStatuses.PendingDelivery;
            entity.DeliveryTimeRange = dto.TimeRange;
        }

        entity.Note = dto.Note;

        return entity;
    }
}
