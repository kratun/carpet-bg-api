using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CarpetBG.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<List<Product>> GetAllAsync(IEnumerable<Guid>? filter = null)
    {
        var query = context.Products.AsNoTracking();
        if (filter != null && filter.Any())
        {
            query = query
                .Where(p => filter.Contains(p.Id));
        }

        var items = await query.ToListAsync();

        return items;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        var query = context.Products.AsQueryable();

        var entity = await query.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);

        return entity;
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        var normalizedName = name.Trim().ToLowerInvariant();

        var entity = await context.Products.FirstOrDefaultAsync(i => i.NormalizedName == normalizedName && !i.IsDeleted);

        return entity;
    }

    public async Task<Product> AddAsync(Product entity)
    {
        context.Products.Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public async Task<Product> UpdateAsync(Product entity)
    {
        context.Products.Update(entity);
        await context.SaveChangesAsync();

        return entity;
    }
}
