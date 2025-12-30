using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Repositories;

public interface IOrderItemRepository
{
    Task<OrderItem> AddAsync(OrderItem orderItem);
    Task<OrderItem?> GetByIdAsync(Guid id, bool includeDeleted = false, bool includeAdditions = false, bool includeOrder = false);
    Task<OrderItem?> UpdateAsync(OrderItem entity);
}
