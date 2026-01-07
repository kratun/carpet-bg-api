using System.Text.Json;

using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;
using CarpetBG.Infrastructure.Data;

namespace CarpetBG.Infrastructure.Seeders;

public class RoleSeeder(AppDbContext dbContext) : ISeeder
{
    public async Task SeedAsync()
    {
        if (!dbContext.Roles.Any())
        {
            var adminRole = RoleType.Admin.ToString();
            var userRole = RoleType.User.ToString();
            var managerRole = RoleType.Manager.ToString();
            var superAdmin = RoleType.SuperAdmin.ToString();
            dbContext.Roles.AddRange(
                new Role { Id = Guid.NewGuid(), Name = adminRole, NormalizedName = JsonNamingPolicy.CamelCase.ConvertName(adminRole) },
                new Role { Id = Guid.NewGuid(), Name = userRole, NormalizedName = JsonNamingPolicy.CamelCase.ConvertName(userRole) },
                new Role { Id = Guid.NewGuid(), Name = managerRole, NormalizedName = JsonNamingPolicy.CamelCase.ConvertName(managerRole) },
                new Role { Id = Guid.NewGuid(), Name = superAdmin, NormalizedName = JsonNamingPolicy.CamelCase.ConvertName(superAdmin) }
            );
            await dbContext.SaveChangesAsync();
        }
    }
}
