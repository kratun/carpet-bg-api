using CarpetBG.Application.DTOs.Addresses;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Factories;

public class AddressFactory : IAddressFactory
{
    public Address CreateFromDto(CreateAddressDto dto)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            DisplayAddress = dto.DisplayAddress,
            UserId = dto.UserId.HasValue ? dto.UserId.Value : Guid.Empty,
        };
    }

    public AddressDto CreateFromEntity(Address address)
    {
        return new()
        {
            DisplayAddress = address.DisplayAddress,
            Id = address.Id,
            PhoneNumber = address.User?.PhoneNumber ?? string.Empty,
            UserFullName = address.User?.FullName ?? string.Empty,
            UserId = address.UserId
        };
    }
}
