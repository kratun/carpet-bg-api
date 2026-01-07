using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.ApplyAuditable();

        builder.HasKey(r => r.Id)
               .HasName("pk_roles");

        builder.Property(r => r.Id)
               .HasColumnName("id")
               .HasColumnType("uuid")
               .IsRequired();

        builder.Property(r => r.Name)
               .HasColumnName("name")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(r => r.NormalizedName)
               .HasColumnName("normalized_name")
               .HasMaxLength(100)
               .IsRequired();

        builder.HasMany(r => r.UserRoles)
               .WithOne(ur => ur.Role)
               .HasForeignKey(ur => ur.RoleId)
               .HasConstraintName("fk_user_roles_role_id")
               .OnDelete(DeleteBehavior.Restrict);

        // Soft-delete-aware unique indexes
        builder.HasIndex(r => r.Name)
               .HasDatabaseName("ux_roles_name_active")
               .IsUnique()
               .HasFilter("is_deleted = false");

        builder.HasIndex(r => r.NormalizedName)
               .HasDatabaseName("ux_roles_normalized_name_active")
               .IsUnique()
               .HasFilter("is_deleted = false");
    }
}
