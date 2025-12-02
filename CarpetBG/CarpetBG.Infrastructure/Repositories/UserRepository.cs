

using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace CarpetBG.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User> AddAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetByPhoneAsync(string phone)
    {
        var user = await context.Users.FirstOrDefaultAsync(i => i.PhoneNumber == phone);

        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id, bool includeDeleted = false)
    {
        var user = await context.Users.FirstOrDefaultAsync(i => i.Id == id && (includeDeleted || !i.IsDeleted));

        return user;
    }
}
