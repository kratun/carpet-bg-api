using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Helpers;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;
using CarpetBG.Shared;

namespace CarpetBG.Application.Services;

public class OrderService(
    IOrderRepository repository,
    IUserRepository userRepository,
    IAddressRepository addressRepository,
    IProductRepository productRepository,
    IOrderFactory orderFactory,
    IValidator<OrderItemDto> orderItemValidator)
    : IOrderService
{
    public async Task<Result<PaginatedList<OrderDto>>> GetFilteredAsync(OrderFilterDto filter)
    {
        var (items, totalCount) = await repository.GetFilteredAsync(filter);

        var paginated = new PaginatedList<OrderDto>(items, totalCount, filter.PageNumber, filter.PageSize);

        return Result<PaginatedList<OrderDto>>.Success(paginated);
    }

    public async Task<Result<OrderDto>> GetByIdAsync(Guid id)
    {
        var order = await repository.GetByIdAsync(id);

        if (order == null)
        {
            return Result<OrderDto>.Failure("No data found");
        }

        return Result<OrderDto>.Success(orderFactory.CreateFromEntity(order));
    }

    public async Task<Result<Guid>> CreateOrderAsync(CreateOrderDto dto)
    {
        var customer = await userRepository.GetByIdAsync(dto.CustomerId);
        if (customer == null)
        {
            return Result<Guid>.Failure("Customer with such data does not exists");
        }

        var address = await addressRepository.GetByIdAsync(dto.PickupAddressId);
        if (address == null || address.UserId != dto.CustomerId)
        {
            return Result<Guid>.Failure("Customer with such address does not exists");
        }

        var error = await ValidateOrderItemsAsync(dto.OrderItems);

        if (!string.IsNullOrEmpty(error))
        {
            return Result<Guid>.Failure("Order items are not valid: " + error);
        }

        // TODO Validate additions
        //if (orderAddition != null && orderAddition.AdditionType == AdditionTypes.AppliedAsPercentage && (orderAddition.Value > 1 || orderAddition.Value <= 0))
        //{
        //    var range = "(0% , 100%]";

        //    throw new ArgumentException($"Total amount addintion should be in range {range}.");
        //}

        //if (!isFree && orderAddition != null && orderAddition.AdditionType == AdditionTypes.AppliedAsPercentage && orderAddition.Value <= 0)
        //{
        //    throw new ArgumentException("Total amount addintion should be greater than zero.");
        //}

        List<IAddition> orderItemAdditions = dto.IsExpress
            ? [new Addition
            {
                Name = "Express",
                NormalizedName = "express",
                AdditionType = AdditionTypes.AppliedAsPercentage,
                Value = 0.5m,
            }]
            : [];
        var order = orderFactory.CreateFromDto(dto, orderItemAdditions);
        var items = order.Items;
        var products = await productRepository.GetAllAsync(items.Where(i => i.ProductId.HasValue).Select(i => i.ProductId!.Value));
        items.ForEach(item =>
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            var price = product != null ? product.Price : decimal.Zero;
            item.Price = price;
        });

        await repository.AddAsync(order);

        return Result<Guid>.Success(order.Id);
    }

    public async Task<Result<Guid>> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto dto)
    {
        var order = await repository.GetByIdAsync(id, needTrackiing: true);
        if (order == null)
        {
            return Result<Guid>.Failure("Ordr was not found");
        }

        var canUpdateManualyOrderStatus = false;
        switch (order.Status)
        {
            case OrderStatuses.PendingPickup:
                if (dto.NextStatus == OrderStatuses.PickupComplete)
                {
                    order.Status = dto.NextStatus;
                    canUpdateManualyOrderStatus = true;
                }
                break;
            case OrderStatuses.PickupComplete:
                if (dto.NextStatus == OrderStatuses.WashingComplete)
                {
                    order.Status = dto.NextStatus;
                    canUpdateManualyOrderStatus = true;
                }
                break;
            default:
                canUpdateManualyOrderStatus = false;
                break;
        }

        if (!canUpdateManualyOrderStatus)
        {
            return Result<Guid>.Failure("Ordr satus can not be updated.");
        }

        await repository.UpdateAsync(order);

        return Result<Guid>.Success(order.Id);
    }

    public async Task<Result<Guid>> RevertOrderStatusAsync(Guid id, UpdateOrderStatusDto dto)
    {
        var order = await repository.GetByIdAsync(id, needTrackiing: true);
        if (order == null)
        {
            return Result<Guid>.Failure("Ordr was not found");
        }

        var canUpdateManualyOrderStatus = false;
        switch (order.Status)
        {
            case OrderStatuses.PendingPickup:
                if (dto.NextStatus == OrderStatuses.New)
                {
                    order.Status = dto.NextStatus;
                    order.PickupDate = null;
                    order.PickupTimeRange = null!;
                    canUpdateManualyOrderStatus = true;
                }
                break;
            case OrderStatuses.PendingDelivery:
                if (dto.NextStatus == OrderStatuses.WashingComplete)
                {
                    order.Status = dto.NextStatus;
                    canUpdateManualyOrderStatus = true;
                }
                break;
            case OrderStatuses.WashinginProgress:
                if (dto.NextStatus == OrderStatuses.PickupComplete)
                {
                    order.Status = dto.NextStatus;
                    canUpdateManualyOrderStatus = true;
                }
                break;
            default:
                canUpdateManualyOrderStatus = false;
                break;
        }

        if (!canUpdateManualyOrderStatus)
        {
            return Result<Guid>.Failure("Order satus can not be reverted.");
        }

        await repository.UpdateAsync(order);

        return Result<Guid>.Success(order.Id);
    }

    public async Task<Result<Guid>> AddDeliveryDataAsync(Guid id, OrderDeliveryDataDto dto)
    {
        var order = await repository.GetByIdAsync(id, needTrackiing: true, includeItems: true);
        if (order == null)
        {
            return Result<Guid>.Failure("Ordr was not found");
        }

        if (order.Status != OrderStatuses.WashingComplete)
        {
            return Result<Guid>.Failure("Order cannot be updated. Incorrect status.");
        }

        var deliveryAddress = await addressRepository.GetByIdAsync(dto.DeliveryAddressId, order.UserId);
        if (deliveryAddress == null)
        {
            return Result<Guid>.Failure("Address was not found");
        }

        orderFactory.UpdateFromDto(dto, order);
        order.Status = OrderStatuses.PendingDelivery;
        await repository.UpdateAsync(order);

        return Result<Guid>.Success(order.Id);

    }

    public async Task<Result<Guid>> ConfirmDeliveryAsync(Guid id, OrderDeliveryConfirmDto dto)
    {
        var order = await repository.GetByIdAsync(id, needTrackiing: true, includeItems: true);
        if (order == null)
        {
            return Result<Guid>.Failure("Order was not found");
        }

        if (order.Status != OrderStatuses.PendingDelivery)
        {
            return Result<Guid>.Failure("Order cannot be updated. Incorrect status.");
        }

        var deliveredItems = dto.DeliveredItems.ToHashSet();
        var orderItems = order.Items;
        var targetItems = order.Items
            .Where(i => deliveredItems.Contains(i.Id))
            .ToList();

        if (orderItems.Count != targetItems.Count)
        {
            return Result<Guid>.Failure("Some of the order items were not found");
        }

        var targetAmount = targetItems
            .Sum(i => OrderItemHelper.CalculateAmount(i.Price, i.Width, i.Height, i.Diagonal, i.Additions.AsEnumerable<IAddition>()));

        if (dto.PaidAmount != targetAmount)
        {
            return Result<Guid>.Failure("Paid amount does not match the target amount");
        }

        targetItems.ForEach(item =>
        {
            item.IsDelivered = true;
        });

        order.Status = OrderStatuses.Completed;

        await repository.UpdateAsync(order);

        return Result<Guid>.Success(id);
    }

    private async Task<string?> ValidateOrderItemsAsync(List<OrderItemDto> items)
    {
        var orderItemsErrors = items
            .Select((item, index) => (item, index))
            .Select(x => (error: orderItemValidator.Validate(x.item), x.index))
            .Where(x => x.error != null)
            .Select((x) => x.error != null ? $"{x.index + 1}. {x.error}" : null)
            .ToList();

        if (orderItemsErrors.Any())
        {
            return string.Join(", ", orderItemsErrors);
        }

        string error = null!;

        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var entity = await productRepository.GetByIdAsync(item.ProductId);
            if (entity == null)
            {
                error = $"The cleaning type for {i + 1}. does not exist.";
                break;
            }
        }

        return error;
    }
}
