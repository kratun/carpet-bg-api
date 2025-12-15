using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Domain.Entities;
using CarpetBG.Shared;

namespace CarpetBG.Application.Services;

public class OrderItemService(
    IOrderItemRepository repository,
    IOrderRepository orderRepository,
    IOrderItemFactory factory,
    IValidator<OrderItemDto> orderItemValidator)
    : IOrderItemService
{
    public async Task<Result<Guid>> AddAsync(OrderItemDto dto, Guid orderId)
    {
        var error = orderItemValidator.Validate(dto);
        if (string.IsNullOrEmpty(error))
        {
            Result<Guid>.Failure($"The order item has error: {error}.");
        }

        var order = await orderRepository.GetByIdAsync(orderId, needTrackiing: false);
        if (order == null)
        {
            return Result<Guid>.Failure("Order not found.");
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
}
