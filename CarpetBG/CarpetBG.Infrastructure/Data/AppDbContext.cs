using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data.Configurations;

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

    public DbSet<Address> Addresses { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderItemAddition> OrderItemAdditions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Addition> Additions { get; set; }
    public DbSet<Product> Products { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var auditDate = _dateTimeProvider.UtcNow;
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = auditDate;
                entry.Entity.UpdatedAt = auditDate;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = auditDate;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new AdditionConfiguration());
    }
}
