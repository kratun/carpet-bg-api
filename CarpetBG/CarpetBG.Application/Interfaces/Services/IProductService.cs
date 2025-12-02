using CarpetBG.Application.DTOs.Products;
using CarpetBG.Shared;

namespace CarpetBG.Application.Interfaces.Services;

public interface IProductService
{
    Task<Result<List<ProductDto>>> GetAllAsync();
    Task<Result<ProductDto>> GetByIdAsync(Guid id);

    Task<Result<Guid>> AddAsync(ProductDto dto);
    Task<Result<Guid>> UpdateAsync(Guid id, ProductDto dto);
}
