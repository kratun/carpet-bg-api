
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CarpetBG.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email)
    {
        return db.Users
            .Include(u => u.Roles)
            .ThenInclude(userRoles => userRoles.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> AddAsync(User user)
    {
        await db.Users.AddAsync(user);

        var result = await db.SaveChangesAsync();

        return result != 0 ? user : null;
    }

    public async Task<List<string>> GetUserRolesByEmailAsync(string email)
    {
        return await db.UserRoles
            .Where(ur => ur.User.Email == email && !ur.IsDeleted && !ur.User.IsDeleted)
            .Select(ur => ur.Role.Name)
            .ToListAsync() ?? [];
    }

    public async Task<UserRole?> AddUserRoleAsync(UserRole userRole)
    {
        await db.UserRoles.AddAsync(userRole);
        var result = await db.SaveChangesAsync();

        return result != 0 ? userRole : null;
    }
}
