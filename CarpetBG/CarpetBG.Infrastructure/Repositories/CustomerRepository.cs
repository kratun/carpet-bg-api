

using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CarpetBG.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    public async Task<Customer> AddAsync(Customer entity)
    {
        context.Customers.Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    public async Task<Customer?> GetByPhoneAsync(string phone)
    {
        var entity = await context.Customers.FirstOrDefaultAsync(i => i.PhoneNumber == phone);

        return entity;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, bool includeDeleted = false)
    {
        var entity = await context.Customers.FirstOrDefaultAsync(i => i.Id == id && (includeDeleted || !i.IsDeleted));

        return entity;
    }
}
