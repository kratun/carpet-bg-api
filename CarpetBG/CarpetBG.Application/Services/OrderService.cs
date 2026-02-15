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
    ICustomerRepository customerRepository,
    IAddressRepository addressRepository,
    IProductRepository productRepository,
    IOrderFactory orderFactory,
    IValidator<OrderItemDto> orderItemValidator,
    IDateTimeProvider dateTimeProvider)
    : IOrderService
{
    public async Task<Result<PaginatedResult<OrderDto>>> GetFilteredAsync(OrderFilterDto filter)
    {
        var (items, totalCount) = await repository.GetFilteredAsync(filter);
        try
        {
            var paginated = orderFactory.CreatePaginatedResult(items, totalCount, filter.PageIndex, filter.PageSize);

            return Result<PaginatedResult<OrderDto>>.Success(paginated);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Result<PaginatedResult<OrderDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<PaginatedResult<OrderDto>>> GetSetupLogisticDataAsync(OrderFilterDto filter)
    {
        var (items, totalCount) = await repository.GetSetupLogisticDataAsync(filter);

        var paginated = orderFactory.CreatePaginatedResult(items, totalCount, filter.PageIndex, filter.PageSize);

        return Result<PaginatedResult<OrderDto>>.Success(paginated);
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
        var customer = await customerRepository.GetByIdAsync(dto.CustomerId);
        if (customer == null)
        {
            return Result<Guid>.Failure("Customer with such data does not exists");
        }

        var address = await addressRepository.GetByIdAsync(dto.PickupAddressId);
        if (address == null || address.CustomerId != dto.CustomerId)
        {
            return Result<Guid>.Failure("Customer with such address does not exists");
        }

        var error = await ValidateOrderItemsAsync(dto.OrderItems, OrderStatuses.New);

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
        try
        {
            await repository.AddAsync(order);
        }
        catch (Exception ex)
        {
            var message = ex.Message;
            throw;
        }


        return Result<Guid>.Success(order.Id);
    }

    public async Task<Result<List<OrderDto>>> SetOrderByAsync(List<OrderDto> request)
    {
        if (request == null || request.Count == 0)
        {
            return Result<List<OrderDto>>.Success(new List<OrderDto>());
        }

        var ids = request.Where(i => i.OrderBy.HasValue).Select(i => i.Id).ToList();

        if (request.Count != ids.Count)
        {
            return Result<List<OrderDto>>.Failure("One or more orders have missing order by value.");
        }

        var entities = await repository.GetByIdsAsync(
            ids,
            needTracking: true,
            includeDeleted: false
        );

        if (entities == null || entities.Count != request.Count)
        {
            return Result<List<OrderDto>>.Failure(
                "One or more orders could not be found."
            );
        }

        var hasInvalidStatus = entities
            .Any(e => e.Status != OrderStatuses.PendingPickup && e.Status != OrderStatuses.PendingDelivery);

        if (hasInvalidStatus)
        {
            return Result<List<OrderDto>>.Failure(
                "One or more orders have invalid status."
            );
        }

        // Build lookup for fast access
        var orderByLookup = request.ToDictionary(x => x.Id, x => x.OrderBy);

        // Update entities (tracked by EF)
        foreach (var entity in entities)
        {
            entity.OrderBy = orderByLookup[entity.Id];
        }

        var updated = await repository.UpdateRangeAsync(entities);

        var result = updated
            .Select(orderFactory.CreateFromEntity)
            .ToList();

        return Result<List<OrderDto>>.Success(result);
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
                    order.PickupDate = dateTimeProvider.UtcNow;
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
            case OrderStatuses.WashingInProgress:
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

        List<OrderStatuses> picupStatses =
        [
            OrderStatuses.PendingPickup,
            OrderStatuses.New
        ];

        List<OrderStatuses> allowedStatuses =
        [
            ..picupStatses,
            OrderStatuses.WashingComplete,
            OrderStatuses.PendingDelivery,
        ];

        if (!allowedStatuses.Contains(order.Status))
        {
            return Result<Guid>.Failure("Order cannot be updated. Incorrect status.");
        }

        var address = await addressRepository.GetByIdAsync(dto.AddressId, order.CustomerId);
        if (address == null)
        {
            return Result<Guid>.Failure("Address was not found");
        }

        orderFactory.UpdateFromDto(dto, order);
        order.Status = picupStatses.Contains(order.Status)
            ? OrderStatuses.PendingPickup
            : OrderStatuses.PendingDelivery;
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
            .Sum(i => OrderItemHelper.CalculateAmount(i.Price, i.Width, i.Height, i.Additions.AsEnumerable<IAddition>()));

        if (dto.PaidAmount != targetAmount)
        {
            return Result<Guid>.Failure("Paid amount does not match the target amount");
        }

        targetItems.ForEach(item =>
        {
            item.IsDelivered = true;
        });

        order.DeliveryDate = dateTimeProvider.UtcNow;
        order.Status = OrderStatuses.Completed;

        await repository.UpdateAsync(order);

        return Result<Guid>.Success(id);
    }

    public async Task<Result<Guid>> CompleteWashingAsync(Guid id)
    {
        var order = await repository.GetByIdAsync(id, needTrackiing: true, includeItems: true);
        if (order == null)
        {
            return Result<Guid>.Failure("Order was not found");
        }

        if (order.Status != OrderStatuses.WashingInProgress)
        {
            return Result<Guid>.Failure("Order cannot be updated. Incorrect status.");
        }


        var orderItems = order.Items;
        var isWashingCoomplete = order.Items
            .All(i => i.Status == OrderItemStatuses.WashingComplete);

        if (!isWashingCoomplete)
        {
            return Result<Guid>.Failure("Some of the order items ahs not completed washing");
        }

        order.Status = OrderStatuses.WashingComplete;

        await repository.UpdateAsync(order);

        return Result<Guid>.Success(id);
    }

    private async Task<string?> ValidateOrderItemsAsync(List<OrderItemDto> items, OrderStatuses orderStatus)
    {
        var orderItemsErrors = items
            .Select((item, index) => (item, index))
            .Select(x => (error: orderItemValidator.Validate(x.item, orderStatus), x.index))
            .Where(x => x.error != null)
            .Select((x) => x.error != null ? $"{x.index + 1}. {x.error}" : null)
            .ToList();

        var separator = ", ";

        if (orderItemsErrors.Count != 0)
        {
            return string.Join(separator, orderItemsErrors);
        }

        List<string> errors = [];

        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var entity = await productRepository.GetByIdAsync(item.ProductId);
            if (entity == null || entity.IsDeleted)
            {
                errors.Add($"The product with ID {item.ProductId} does not exist.");
                continue;
            }
        }

        return errors.Count != 0 ? string.Join(separator, errors) : null;
    }
}
