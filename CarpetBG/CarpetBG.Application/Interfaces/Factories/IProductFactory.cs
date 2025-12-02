using CarpetBG.Application.DTOs.Products;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Factories;

public interface IProductFactory
{
    Product CreateFromDto(ProductDto dto);
    Product UpdateFromDto(Product entity, ProductDto dto);
    ProductDto CreateFromEntity(Product entity);
}
