using CarpetBG.Application.DTOs.Addresses;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Factories;

public interface IAddressFactory
{
    Address CreateFromDto(CreateAddressDto dto);
    AddressDto CreateFromEntity(Address address);
}
