using CarpetBG.Application.DTOs.Addresses;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Factories;

public class AddressFactory : BaseFactory, IAddressFactory
{
    public Address CreateFromDto(CreateAddressDto dto)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            DisplayAddress = dto.DisplayAddress,
            CustomerId = dto.CustomerId.HasValue ? dto.CustomerId.Value : Guid.Empty,
        };
    }

    public AddressDto CreateFromEntity(Address address)
    {
        return new()
        {
            DisplayAddress = address.DisplayAddress,
            Id = address.Id,
            PhoneNumber = address.Customer?.PhoneNumber ?? string.Empty,
            CustomerFullName = address.Customer?.FullName ?? string.Empty,
            CustomerId = address.CustomerId
        };
    }
}
