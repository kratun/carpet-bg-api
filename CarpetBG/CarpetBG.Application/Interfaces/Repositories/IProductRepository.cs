using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(IEnumerable<Guid>? filter = null);
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetByNameAsync(string name);
    Task<Product> AddAsync(Product entity);
    Task<Product> UpdateAsync(Product entity);
}
