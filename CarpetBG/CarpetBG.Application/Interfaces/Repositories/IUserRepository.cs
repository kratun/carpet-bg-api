using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> AddAsync(User user);
    Task<UserRole?> AddUserRoleAsync(UserRole userRole);
    Task<List<string>> GetUserRolesByEmailAsync(string email);
}
