using CarpetBG.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Configurations;

public class AdditionConfiguration : IEntityTypeConfiguration<Addition>
{
    public void Configure(EntityTypeBuilder<Addition> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasMany(a => a.OrderItemAdditions)
            .WithOne(oia => oia.Addition)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(o => o.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(o => o.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
