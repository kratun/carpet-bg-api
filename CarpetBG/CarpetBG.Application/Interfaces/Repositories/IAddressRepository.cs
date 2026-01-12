using CarpetBG.Application.DTOs.Addresses;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Repositories;

public interface IAddressRepository
{
    Task<(List<AddressDto> Items, int TotalCount)> GetFilteredAsync(AddressFilterDto filter, bool includeDeleted = false);
    Task<Address?> GetByIdAsync(Guid id, bool includeDeleted = false, bool needTrackiing = false);
    Task<Address?> GetByIdAsync(Guid id, Guid customerId, bool includeDeleted = false, bool needTrackiing = false);
    Task<Address> AddAsync(Address address);
}
