using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Domain.Constants;

namespace CarpetBG.Application.Validations;

public class OrderItemDtoValidator : IValidator<OrderItemDto>
{
    public string? Validate(OrderItemDto dto)
    {
        if (dto == null)
        {
            return "OrderItemDto cannot be null.";
        }

        if (!dto.Width.HasValue)
        {
            return "Width is required.";
        }

        if (dto.Width.Value <= OrderItemValidationConstants.DefaultMeasurementValue)
        {
            return "Width must be greater than 0";
        }

        if (dto.Height.HasValue && dto.Height.Value <= OrderItemValidationConstants.DefaultMeasurementValue)
        {
            return "Height must be greater than 0";
        }

        // TODO Add Addition validator
        return null;
    }
}
