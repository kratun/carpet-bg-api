using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Helpers;
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
    public OrderPrintDto MapToPrintModel(Order order, int minRows = 0)
    {
        var isExpress = false;
        var items = new List<OrderPrintItemDto>();
        foreach (var item in order.Items)
        {
            var additions = item.Additions;
            if (additions?.Any(i => i.NormalizedName == "express") == true)
            {
                isExpress = true;
            }

            var printItem = new OrderPrintItemDto
            {
                Quantity = CommonHelper.ConvertToMeasurment(OrderItemHelper.CalculateQuantity(item.Width, item.Height)),
                Note = item.Note,
                Width = CommonHelper.ConvertToMeasurment(item.Width),
                Height = CommonHelper.ConvertToMeasurment(item.Height),
                UnitPrice = CommonHelper.ConvertToMoney(OrderItemHelper.ApplyAdditions(item.Price, item.Additions)),
                Amount = CommonHelper.ConvertToMoney(OrderItemHelper.CalculateAmount(isExpress ? item.Product.ExpressServicePrice : item.Price, item.Width, item.Height, item.Additions)),
                ProductName = item.Product.Name,
                Measurment = OrderItemHelper.GetMeasurmentData(item.Width, item.Height),
                Status = CommonHelper.ConvertToString(item.Status),
                Id = item.Id,
            };

            items.Add(printItem);
        }

        var deliveryAddress = order.DeliveryAddressId.HasValue && order.DeliveryAddressId.Value != order.PickupAddressId
            ? order.DeliveryAddress.DisplayAddress
            : "Същият като адреса за вземане";

        var model = new OrderPrintDto
        {
            OrderNumber = CommonHelper.ConvertToString(order.OrderNumber),
            CreatedAt = CommonHelper.ConvertToDate(order.CreatedAt),
            CustomerFullName = order.Customer.FullName,
            DeliveryAddress = deliveryAddress,
            DeliveryDate = CommonHelper.ConvertToDateWithTime(order.DeliveryDate),
            DeliveryTimeRange = order.DeliveryTimeRange,
            IsExpress = isExpress,
            Note = order.Note,
            PickupAddress = order.PickupAddress.DisplayAddress,
            PhoneNumber = order.Customer.PhoneNumber,
            PickupDate = CommonHelper.ConvertToDateWithTime(order.PickupDate),
            PickupTimeRange = order.PickupTimeRange,
            Status = CommonHelper.ConvertToString(order.Status),
            OrderItems = items,
        };

        // ⭐ PAD EMPTY ROWS
        int missing = minRows - model.OrderItems.Count;

        if (missing > 0)
        {
            for (int i = 0; i < missing; i++)
            {
                model.OrderItems.Add(new OrderPrintItemDto
                {
                    IsPlaceholder = true
                });
            }
        }

        return model;
    }
}
