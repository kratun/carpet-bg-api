using CarpetBG.Domain.Constants;
using CarpetBG.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(UserValidationConstants.FullNameMaxLength);

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(UserValidationConstants.PhoneNumberMaxLength);

        builder.HasMany(c => c.Addresses)
            .WithOne(c => c.User)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

