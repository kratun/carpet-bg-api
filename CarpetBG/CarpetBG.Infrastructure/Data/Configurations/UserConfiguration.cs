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
               .HasColumnName("user_id")
               .HasColumnType("uuid")
               .IsRequired();

        builder.Property(u => u.Email)
               .HasColumnName("email")
               .HasMaxLength(256)
               .IsRequired();

        // Soft-delete-aware unique email
        builder.HasIndex(u => u.Email)
               .IsUnique()
               .HasDatabaseName("ux_users_email_active")
               .HasFilter("is_deleted = false");
    }
}
