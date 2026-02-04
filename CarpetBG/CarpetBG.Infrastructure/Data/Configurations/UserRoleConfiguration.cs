using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.ApplyAuditable();

        builder
            .HasKey(x => x.Id)
            .HasName("pk_user_roles");

        builder.Property(u => u.Id)
               .HasColumnName("user_role_id")
               .HasColumnType("uuid")
               .IsRequired();

        builder
            .Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .Property(x => x.RoleId)
            .HasColumnName("role_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .HasOne(x => x.User)
            .WithMany(nameof(User.Roles))
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("fk_user_roles_user_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId)
            .HasConstraintName("fk_user_roles_role_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
