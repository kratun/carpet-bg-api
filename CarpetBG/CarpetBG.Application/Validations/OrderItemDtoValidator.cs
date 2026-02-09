using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Domain.Constants;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.Validations;

public class OrderItemDtoValidator : IValidator<OrderItemDto>
{
    public string? Validate(OrderItemDto dto, OrderStatuses? orderStatus)
    {
        if (dto == null)
        {
            return "OrderItemDto cannot be null.";
        }

        bool hasWidth = dto.Width.HasValue;
        bool hasHeight = dto.Height.HasValue;

        if (hasWidth ^ hasHeight)
        {
            return "Width and Height must be provided together.";
        }
        if (orderStatus.HasValue && orderStatus > OrderStatuses.New && !hasWidth && !hasHeight)
        {
            return "Width and Height are required.";
        }

        if (dto.Width.HasValue && dto.Width.Value <= OrderItemValidationConstants.DefaultMeasurementValue)
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
