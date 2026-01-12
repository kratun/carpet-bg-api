using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.ApplyAuditable();

        builder.HasKey(u => u.Id)
               .HasName("pk_users");

        builder.Property(u => u.Id)
               .HasColumnName("id")
               .HasColumnType("uuid")
               .IsRequired();

        builder.Property(u => u.Email)
               .HasColumnName("email")
               .HasMaxLength(256)
               .IsRequired();

        builder.Property(u => u.CustomerId)
               .HasColumnName("customer_id")
               .HasColumnType("uuid");

        builder.HasMany(u => u.UserRoles)
               .WithOne(ur => ur.User)
               .HasForeignKey(ur => ur.UserId)
               .HasConstraintName("fk_user_roles_user_id")
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Customer)
               .WithOne()
               .HasForeignKey<User>(u => u.CustomerId)
               .HasConstraintName("fk_users_customer_id")
               .OnDelete(DeleteBehavior.Restrict);

        // Soft-delete-aware unique email
        builder.HasIndex(u => u.Email)
               .IsUnique()
               .HasDatabaseName("ux_users_email_active")
               .HasFilter("is_deleted = false");

        // indeces
        builder
            .HasIndex(u => u.Email)
            .HasDatabaseName("ix_users_email");
    }
}
