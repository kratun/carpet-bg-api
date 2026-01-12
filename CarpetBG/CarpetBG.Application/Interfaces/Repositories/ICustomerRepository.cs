using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByPhoneAsync(string phone);
    Task<Customer?> GetByIdAsync(Guid id, bool includeDeleted = false);
    Task<Customer> AddAsync(Customer entity);
}
