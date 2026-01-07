using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data.Extensions;

using Microsoft.EntityFrameworkCore;

namespace CarpetBG.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public AppDbContext(DbContextOptions<AppDbContext> options, IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Addition> Additions => Set<Addition>();
    public DbSet<User> Users => Set<User>();

    public override int SaveChanges()
    {
        ApplyAuditData();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditData();

        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditData()
    {
        var auditDate = _dateTimeProvider.UtcNow;
        var loggedUserId = null ?? "system";
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = auditDate;
                entry.Entity.CreatedBy = loggedUserId;
                entry.Entity.UpdatedAt = auditDate;
                entry.Entity.UpdatedBy = loggedUserId;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = auditDate;
                entry.Entity.UpdatedBy = loggedUserId;

                entry.Property(e => e.CreatedAt).IsModified = false;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurations();
    }
}
