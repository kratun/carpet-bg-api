using CarpetBG.Application.DTOs.Products;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Factories;

public class ProductFactory() : BaseFactory, IProductFactory
{
    public Product CreateFromDto(ProductDto dto)
    {
        var id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id;
        return new()
        {
            Id = id,
            Description = dto.Description,
            Name = dto.Name,
            NormalizedName = dto.Name.Trim().ToLowerInvariant(),
            Price = dto.Price,
            ExpressServicePrice = dto.ExpressServicePrice,
            OrderBy = dto.OrderBy,
        };
    }

    public ProductDto CreateFromEntity(Product entity)
    {

        return new()
        {
            Id = entity.Id,
            Description = entity.Description,
            ExpressServicePrice = entity.ExpressServicePrice,
            Name = entity.Name,
            Price = entity.Price,
            OrderBy = entity.OrderBy,
        };
    }

    public Product UpdateFromDto(Product entity, ProductDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Price = dto.Price;
        entity.ExpressServicePrice = dto.ExpressServicePrice;
        entity.OrderBy = dto.OrderBy;
        entity.NormalizedName = dto.Name.Trim().ToLowerInvariant();

        return entity;
    }
}
