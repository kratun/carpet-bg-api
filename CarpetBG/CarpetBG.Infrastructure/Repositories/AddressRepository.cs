
using CarpetBG.Application.DTOs.Addresses;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CarpetBG.Infrastructure.Repositories;

public class AddressRepository(AppDbContext context) : IAddressRepository
{
    public async Task<(List<AddressDto> Items, int TotalCount)> GetFilteredAsync(AddressFilterDto filter, bool includeDeleted = false)
    {
        var query = context.Addresses.AsNoTracking();

        if (!includeDeleted)
        {
            query = query.Where(i => !i.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTermNormalized = filter.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(i =>
                (i.User != null && i.User.PhoneNumber.Contains(searchTermNormalized, StringComparison.InvariantCultureIgnoreCase))
                || i.DisplayAddress.Contains(searchTermNormalized, StringComparison.InvariantCultureIgnoreCase));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(i => new AddressDto
            {
                Id = i.Id,
                DisplayAddress = i.DisplayAddress,
                PhoneNumber = i.User.PhoneNumber,
                UserFullName = i.User.FullName,
                UserId = i.UserId
            })
            .ToListAsync();

        return (items, totalCount);
    }
    public async Task<Address?> GetByIdAsync(Guid id, bool includeDeleted = false, bool needTrackiing = false)
    {
        var query = context.Addresses
            .Include(i => i.User)
            .AsQueryable();

        if (!needTrackiing)
        {
            query = query.AsNoTracking();
        }

        var address = await query.FirstOrDefaultAsync(i => i.Id == id && (includeDeleted || !i.IsDeleted));

        return address;
    }

    public async Task<Address?> GetByIdAsync(Guid id, Guid userId, bool includeDeleted = false, bool needTrackiing = false)
    {
        var query = context.Addresses
            .Include(i => i.User)
            .AsQueryable();

        if (!needTrackiing)
        {
            query = query.AsNoTracking();
        }

        var address = await query.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId && (includeDeleted || !i.IsDeleted));

        return address;
    }

    public async Task<Address> AddAsync(Address address)
    {
        context.Addresses.Add(address);
        await context.SaveChangesAsync();

        return address;
    }
}
