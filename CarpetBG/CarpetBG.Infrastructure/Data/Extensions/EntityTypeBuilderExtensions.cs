using CarpetBG.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<T> ApplyAuditable<T>(
        this EntityTypeBuilder<T> builder)
        where T : class
    {
        // If T does NOT implement IAuditable → no-op
        if (!typeof(IAuditable).IsAssignableFrom(typeof(T)))
        {
            return builder;
        }

        builder.Property(nameof(IAuditable.CreatedAt))
               .HasColumnName("created_at")
               .HasColumnType("timestamp with time zone")
               .IsRequired();

        builder.Property(nameof(IAuditable.CreatedBy))
               .HasColumnName("created_by")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(nameof(IAuditable.UpdatedAt))
               .HasColumnName("updated_at")
               .HasColumnType("timestamp with time zone")
               .IsRequired();

        builder.Property(nameof(IAuditable.UpdatedBy))
               .HasColumnName("updated_by")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(nameof(IAuditable.IsDeleted))
               .HasColumnName("is_deleted")
               .HasDefaultValue(false)
               .IsRequired();

        return builder;
    }
}
