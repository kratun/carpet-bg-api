using CarpetBG.Domain.Constants;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses");

        builder.ApplyAuditable();

        builder.HasKey(a => a.Id)
               .HasName("pk_addresses");

        builder.Property(a => a.Id)
               .HasColumnName("id")
               .HasColumnType("uuid")
               .IsRequired();

        builder.Property(a => a.DisplayAddress)
               .HasColumnName("display_address")
               .HasMaxLength(CustomerValidationConstants.DisplayAddressMaxLength)
               .IsRequired();

        builder.Property(a => a.CustomerId)
               .HasColumnName("customer_id")
               .HasColumnType("uuid")
               .IsRequired();

        builder.HasOne(a => a.Customer)
               .WithMany(c => c.Addresses)
               .HasForeignKey(a => a.CustomerId)
               .HasConstraintName("fk_addresses_customer_id")
               .OnDelete(DeleteBehavior.Restrict);

        // Optional indexes
        builder.HasIndex(a => a.CustomerId)
               .HasDatabaseName("ix_addresses_customer_id");
    }
}
