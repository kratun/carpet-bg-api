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

        if (dto.Diagonal <= OrderItemValidationConstants.DefaultMeasurementValue
            && dto.Height <= OrderItemValidationConstants.DefaultMeasurementValue
            && dto.Width <= OrderItemValidationConstants.DefaultMeasurementValue)
        {
            return "Height and Width must be greater than 0, or Diagonal must be greater than 0.";
        }

        if (dto.Height > OrderItemValidationConstants.DefaultMeasurementValue
            && dto.Width > OrderItemValidationConstants.DefaultMeasurementValue
            && dto.Diagonal > OrderItemValidationConstants.DefaultMeasurementValue)
        {
            return "Diagonal must be empty.";
        }

        if (dto.Diagonal > OrderItemValidationConstants.DefaultMeasurementValue
            && (dto.Height > OrderItemValidationConstants.DefaultMeasurementValue
                || dto.Width > OrderItemValidationConstants.DefaultMeasurementValue))
        {
            return "Height or Width must be empty.";
        }

        // TODO Add Addition validator
        return null;
    }
}
