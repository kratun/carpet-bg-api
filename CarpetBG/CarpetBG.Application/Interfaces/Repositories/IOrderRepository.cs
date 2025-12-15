using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<(List<OrderDto> Items, int TotalCount)> GetFilteredAsync(OrderFilterDto filter, bool includeDeleted = false);
    Task<Order?> GetByIdAsync(Guid id, bool includeDeleted = false, bool needTrackiing = false, bool includeItems = false);
    Task<Order> AddAsync(Order order);
    Task<Order> UpdateAsync(Order order);
}
