using CarpetBG.Application.DTOs.Addresses;
using CarpetBG.Shared;

namespace CarpetBG.Application.Interfaces.Services;

public interface IAddressService
{
    Task<Result<PaginatedList<AddressDto>>> GetFilteredAsync(AddressFilterDto filter);
    Task<Result<AddressDto>> GetByIdAsync(Guid addressId);
    Task<Result<AddressDto>> CreateAddressAsync(CreateAddressDto dto);
}
