using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CarpetBG.Infrastructure.Repositories;

public class OrderItemRepository(AppDbContext context) : IOrderItemRepository
{
    public async Task<OrderItem> AddAsync(OrderItem entity)
    {
        context.OrderItems.Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public async Task<OrderItem?> GetByIdAsync(Guid id, bool includeDeleted = false, bool includeAdditions = false, bool includeOrder = false)
    {
        var query = context.OrderItems.AsQueryable();

        if (includeAdditions)
        {
            query = query
                .Include(i => i.Additions);
        }

        if (includeOrder)
        {
            query = query
                .Include(i => i.Order);
        }

        var entity = await query
            .FirstOrDefaultAsync(i => i.Id == id && (!includeDeleted || i.IsDeleted));

        return entity;
    }

    public async Task<OrderItem?> UpdateAsync(OrderItem entity)
    {
        context.OrderItems.Update(entity);

        var result = await context.SaveChangesAsync();

        return result > 0 ? entity : null;
    }
}
