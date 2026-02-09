using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;
using CarpetBG.Shared;

namespace CarpetBG.Application.Services;

public class OrderItemService(
    IOrderItemRepository repository,
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IOrderItemFactory factory,
    IValidator<OrderItemDto> orderItemValidator)
    : IOrderItemService
{
    public async Task<Result<Guid>> AddAsync(OrderItemDto dto, Guid orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId, needTrackiing: false);
        if (order == null)
        {
            return Result<Guid>.Failure("Order not found.");
        }

        var error = orderItemValidator.Validate(dto, order.Status);
        if (string.IsNullOrEmpty(error))
        {
            Result<Guid>.Failure($"The order item has error: {error}.");
        }

        var additions = dto.Additions.Select(a => new Addition
        {
            Name = a.Name,
            AdditionType = a.AdditionType,
            NormalizedName = a.NormalizedName,
            Value = a.Value,
        }).ToList<IAddition>();

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

        var entity = factory.CreateFromDto(dto, orderId, additions);

        await repository.AddAsync(entity);

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result<Guid>> CompleteWashingAsync(Guid id, Guid orderId)
    {
        var orderItem = await repository.GetByIdAsync(id, includeOrder: true);
        if (orderItem == null || orderItem.IsDeleted)
        {
            return Result<Guid>.Failure("Order item not found.");
        }

        if (orderItem.OrderId != orderId || orderItem.Order.IsDeleted)
        {
            return Result<Guid>.Failure("Order not found.");
        }

        var error = orderItemValidator.Validate(factory.CreateFromEntity(orderItem), orderItem.Order.Status);
        if (!string.IsNullOrEmpty(error))
        {
            return Result<Guid>.Failure("Order item is not valid: " + error);
        }

        var entity = factory.CreateFromDto(OrderItemStatuses.WashingComplete, orderItem, OrderStatuses.WashingInProgress);

        var result = await repository.UpdateAsync(entity);

        if (result == null)
        {
            return Result<Guid>.Failure("Order Item was not updated");
        }

        return Result<Guid>.Success(entity.Id);
    }

    public async Task<Result<Guid>> UpdateAsync(Guid id, OrderItemDto dto, Guid orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId, needTrackiing: false);
        if (order == null || order.IsDeleted)
        {
            return Result<Guid>.Failure("Order not found.");
        }

        var error = orderItemValidator.Validate(dto, order.Status);
        if (!string.IsNullOrEmpty(error))
        {
            Result<Guid>.Failure($"The order item has error: {error}.");
        }

        // TODO: check is the satus available for updates
        var orderItem = await repository.GetByIdAsync(id);
        if (orderItem == null || orderItem.IsDeleted || orderItem.OrderId != orderId)
        {
            return Result<Guid>.Failure("Order item not found.");
        }

        if (orderItem.Status != dto.Status)
        {
            return Result<Guid>.Failure($"Order item status cannot be updated from {orderItem.Status} to {dto.Status}.");
        }

        var additions = dto.Additions.Select(a => new Addition
        {
            Name = a.Name,
            AdditionType = a.AdditionType,
            NormalizedName = a.NormalizedName,
            Value = a.Value,
        }).ToList<IAddition>();

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

        // TODO: Apply to cuerrnet entity
        var product = await productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
        {
            return Result<Guid>.Failure("Product not found.");
        }

        var entity = factory.CreateFromDto(orderItem, dto, product, orderAdditions: additions);

        var result = await repository.UpdateAsync(entity);

        if (result == null)
        {
            return Result<Guid>.Failure("Order Item was not updated");
        }

        return Result<Guid>.Success(entity.Id);
    }
}
