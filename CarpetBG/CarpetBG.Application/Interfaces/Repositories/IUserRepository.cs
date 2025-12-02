using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByPhoneAsync(string phone);
    Task<User?> GetByIdAsync(Guid id, bool includeDeleted = false);
    Task<User> AddAsync(User user);
}
