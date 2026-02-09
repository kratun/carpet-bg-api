using CarpetBG.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace CarpetBG.Infrastructure.Data;

public interface IAppDbContext
{
    // DbSets (only aggregates your app works with)
    DbSet<Customer> Customers { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Product> Products { get; }
    DbSet<Address> Addresses { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<Addition> Additions { get; }
    DbSet<User> Users { get; }

    // Persistence
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
