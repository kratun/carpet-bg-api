using CarpetBG.Application.DTOs.Users;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Domain.Constants;

namespace CarpetBG.Application.Validations;

public class CreateUserDtoValidator : IValidator<CreateUserDto>
{
    public string? Validate(CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            return "Full name is required.";
        }

        if (dto.FullName.Length > UserValidationConstants.FullNameMaxLength)
        {
            return $"Full name must be at most {UserValidationConstants.FullNameMaxLength} characters.";
        }

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            return "Phone number is required.";
        }

        if (!dto.PhoneNumber.All(char.IsDigit))
        {
            return "Phone number must contain digits only.";
        }

        if (dto.PhoneNumber.Length != UserValidationConstants.PhoneNumberMaxLength)
        {
            return $"Phone number must be {UserValidationConstants.PhoneNumberMaxLength} digits. 0XXX XXX XXX";
        }

        return null;
    }
}
