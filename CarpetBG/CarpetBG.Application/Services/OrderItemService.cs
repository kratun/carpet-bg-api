using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
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

        var entity = factory.CreateFromDto(dto, orderId);

        await repository.AddAsync(entity);

        return Result<Guid>.Success(entity.Id);
    }
}
