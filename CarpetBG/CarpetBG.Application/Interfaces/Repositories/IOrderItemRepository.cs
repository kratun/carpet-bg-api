using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Repositories;

public interface IOrderItemRepository
{
    Task<OrderItem> AddAsync(OrderItem orderItem);
}
