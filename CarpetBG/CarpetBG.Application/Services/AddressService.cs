

using CarpetBG.Application.DTOs.Addresses;
using CarpetBG.Application.DTOs.Customers;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Domain.Constants;
using CarpetBG.Shared;

namespace CarpetBG.Application.Services;

public class AddressService(IAddressRepository addressRepository, ICustomerRepository customerRepository, IAddressFactory addressFactory, ICustomerFactory customerFactory, IValidator<CreateCustomerDto> createCustomerValidator) : IAddressService
{
    public async Task<Result<AddressDto>> CreateAddressAsync(CreateAddressDto dto)
    {
        // Validation        
        if (dto.DisplayAddress?.Length > CustomerValidationConstants.DisplayAddressMaxLength)
        {
            var error = $"The address lenth {dto.DisplayAddress.Length} exceed the max size {CustomerValidationConstants.DisplayAddressMaxLength}";
            return Result<AddressDto>.Failure(error);
        }

        var customer = await customerRepository.GetByPhoneAsync(dto.PhoneNumber);
        if (customer == null)
        {
            var customerDto = new CreateCustomerDto { FullName = dto.CustomerFullName, PhoneNumber = dto.PhoneNumber };
            var error = createCustomerValidator.Validate(customerDto);
            if (error is not null)
            {
                return Result<AddressDto>.Failure(error);
            }

            customer = await customerRepository.AddAsync(customerFactory.CreateFromDto(customerDto));
        }
        dto.CustomerId = customer.Id;
        var address = addressFactory.CreateFromDto(dto);
        try
        {
            await addressRepository.AddAsync(address);
        }
        catch (Exception ex)
        {
            var message = ex.Message;

        }

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

        var paginated = addressFactory.CreatePaginatedResult(items, totalCount, filter.PageIndex, filter.PageSize);

        return Result<PaginatedResult<AddressDto>>.Success(paginated);
    }
}
