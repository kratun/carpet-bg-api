

using CarpetBG.Application.DTOs.Addresses;
using CarpetBG.Application.DTOs.Users;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Shared;

namespace CarpetBG.Application.Services;

public class AddressService(IAddressRepository addressRepository, IUserRepository userRepository, IAddressFactory addressFactory, IUserFactory userFactory, IValidator<CreateUserDto> userValidator) : IAddressService
{
    public async Task<Result<AddressDto>> CreateAddressAsync(CreateAddressDto dto)
    {
        // Validation

        var user = await userRepository.GetByPhoneAsync(dto.PhoneNumber);
        if (user == null)
        {
            var userDto = new CreateUserDto { FullName = dto.UserFullName, PhoneNumber = dto.PhoneNumber };
            var error = userValidator.Validate(userDto);
            if (error is not null)
            {
                return Result<AddressDto>.Failure(error);
            }

            user = await userRepository.AddAsync(userFactory.CreateFromDto(userDto));
        }
        dto.UserId = user.Id;
        var address = addressFactory.CreateFromDto(dto);
        await addressRepository.AddAsync(address);
        return Result<AddressDto>.Success(addressFactory.CreateFromEntity(address));
    }

    public async Task<Result<AddressDto>> GetByIdAsync(Guid addressId)
    {
        var address = await addressRepository.GetByIdAsync(addressId);

        if (address == null)
        {
            return Result<AddressDto>.Failure("No data found");
        }

        return Result<AddressDto>.Success(addressFactory.CreateFromEntity(address));
    }

    public async Task<Result<PaginatedResult<AddressDto>>> GetFilteredAsync(AddressFilterDto filter)
    {
        var (items, totalCount) = await addressRepository.GetFilteredAsync(filter);

        var paginated = new PaginatedResult<AddressDto>(items, totalCount, filter.PageIndex, filter.PageSize);

        return Result<PaginatedResult<AddressDto>>.Success(paginated);
    }
}
