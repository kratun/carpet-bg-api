using CarpetBG.Application.DTOs.Products;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.Validations;

public class ProductDtoValidator : IValidator<ProductDto>
{
    public string? Validate(ProductDto dto, OrderStatuses? orderStatus = null)
    {
        if (dto == null)
        {
            return "ProductDto cannot be null.";
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return "Product name is required.";
        }

        if (dto.Price <= 0)
        {
            return "Product price must be greater than zero.";
        }

        if (dto.ExpressServicePrice <= 0)
        {
            return "Express service price must be greater than zero.";
        }

        return null;
    }
}
