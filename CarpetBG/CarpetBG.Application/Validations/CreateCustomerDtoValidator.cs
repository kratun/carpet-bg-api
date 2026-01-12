using CarpetBG.Application.DTOs.Customers;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Domain.Constants;

namespace CarpetBG.Application.Validations;

public class CreateCustomerDtoValidator : IValidator<CreateCustomerDto>
{
    public string? Validate(CreateCustomerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            return "Full name is required.";
        }

        if (dto.FullName.Length > CustomerValidationConstants.FullNameMaxLength)
        {
            return $"Full name must be at most {CustomerValidationConstants.FullNameMaxLength} characters.";
        }

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            return "Phone number is required.";
        }

        if (!dto.PhoneNumber.All(char.IsDigit))
        {
            return "Phone number must contain digits only.";
        }

        if (dto.PhoneNumber.Length != CustomerValidationConstants.PhoneNumberMaxLength)
        {
            return $"Phone number must be {CustomerValidationConstants.PhoneNumberMaxLength} digits. 0XXX XXX XXX";
        }

        return null;
    }
}
