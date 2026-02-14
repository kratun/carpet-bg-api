using CarpetBG.Infrastructure.Data;
using CarpetBG.Infrastructure.Options;

using Microsoft.Extensions.Options;

namespace CarpetBG.Infrastructure.Seeders;

public class UserSeeder(AppDbContext dbContext, IOptions<SuperAdminOptions> options) : ISeeder
{
    public async Task SeedAsync()
    {
        await SeedSuperAdminAsync();
        await SeedUserAsync();
    }

    private async Task SeedUserAsync()
    {
        var userEmail = "mitevdv+carpetbg.user@gmail.com";
        var user = dbContext.Users.FirstOrDefault(u => u.Email == userEmail);

        var hasChanges = false;

        if (user == null)
        {
            user = new User(userEmail);
            dbContext.Users.Add(user);
            hasChanges = true;
        }

        if (user.IsDeleted)
        {
            user.IsDeleted = false;
            hasChanges = true;
        }

        var allRoles = dbContext.Roles.Where(r => !r.IsDeleted && r.NormalizedName == "user").ToList();
        var currentUserRoles = dbContext.UserRoles.Where(ur => ur.UserId == user.Id).ToList();

        // Add missing roles
        foreach (var role in allRoles)
        {
            var userRole = currentUserRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
            if (userRole == null)
            {
                user.AddRole(role.Id);
                hasChanges = true;
            }
            else if (userRole.IsDeleted)
            {
                userRole.IsDeleted = false;
                hasChanges = true;
            }
        }

        // Soft delete roles that have been deleted
        var deletedRoles = dbContext.Roles.Where(r => r.IsDeleted).Select(r => r.Id).ToList();
        foreach (var userRole in currentUserRoles)
        {
            if (deletedRoles.Contains(userRole.RoleId) && !userRole.IsDeleted)
            {
                userRole.IsDeleted = true;
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedSuperAdminAsync()
    {
        var superAdminEmail = options.Value.Email;
        var superAdminUser = dbContext.Users.FirstOrDefault(u => u.Email == superAdminEmail);
        var hasChanges = false;

        if (superAdminUser == null)
        {
            superAdminUser = new User(superAdminEmail);
            dbContext.Users.Add(superAdminUser);
            hasChanges = true;
        }

        if (superAdminUser.IsDeleted)
        {
            superAdminUser.IsDeleted = false;
            hasChanges = true;
        }

        var allRoles = dbContext.Roles.Where(r => !r.IsDeleted).ToList();
        var currentUserRoles = dbContext.UserRoles.Where(ur => ur.UserId == superAdminUser.Id).ToList();

        // Add missing roles
        foreach (var role in allRoles)
        {
            var userRole = currentUserRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
            if (userRole == null)
            {
                superAdminUser.AddRole(role.Id);
                hasChanges = true;
            }
            else if (userRole.IsDeleted)
            {
                userRole.IsDeleted = false;
                hasChanges = true;
            }
        }

        // Soft delete roles that have been deleted
        var deletedRoles = dbContext.Roles.Where(r => r.IsDeleted).Select(r => r.Id).ToList();
        foreach (var userRole in currentUserRoles)
        {
            if (deletedRoles.Contains(userRole.RoleId) && !userRole.IsDeleted)
            {
                userRole.IsDeleted = true;
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
