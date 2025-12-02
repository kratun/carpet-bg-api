using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data;

namespace CarpetBG.Infrastructure.Repositories;

public class OrderItemRepository(AppDbContext context) : IOrderItemRepository
{
    public async Task<OrderItem> AddAsync(OrderItem entity)
    {
        context.OrderItems.Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }
}
