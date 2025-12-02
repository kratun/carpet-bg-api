using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
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

        var order = orderFactory.CreateFromDto(dto);

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
            case OrderStatuses.PendingDelivery:
                if (dto.NextStatus == OrderStatuses.DeliveryComplete)
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
            return Result<Guid>.Failure("Ordr satus can not be updated");
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
            return Result<Guid>.Failure("Ordr satus can not be updated");
        }

        await repository.UpdateAsync(order);

        return Result<Guid>.Success(order.Id);
    }

    public async Task<Result<Guid>> AddDeliveryDataAsync(Guid id, OrderDeliveryDataDto dto)
    {
        var order = await repository.GetByIdAsync(id, needTrackiing: true);
        if (order == null)
        {
            return Result<Guid>.Failure("Ordr was not found");
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
