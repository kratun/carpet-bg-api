using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(IEnumerable<Guid>? filter = null);
    Task<Product?> GetByIdAsync(Guid id, bool includeDeleted = false);
    Task<Product?> GetByNameAsync(string name, bool includeDeleted = false);
    Task<Product> AddAsync(Product entity);
    Task<Product> UpdateAsync(Product entity);
}
