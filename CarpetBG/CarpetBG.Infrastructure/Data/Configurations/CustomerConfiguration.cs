using CarpetBG.Domain.Constants;
using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.ApplyAuditable();

        builder
            .HasKey(c => c.Id)
            .HasName("pk_customers");

        builder
            .Property(c => c.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .Property(c => c.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(CustomerValidationConstants.FullNameMaxLength)
            .IsRequired();

        builder
            .Property(c => c.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(CustomerValidationConstants.PhoneNumberMaxLength)
            .IsRequired();

        builder
            .Property(c => c.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uuid");

        builder
            .HasMany(c => c.Addresses)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .HasConstraintName("fk_addresses_customer_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .HasConstraintName("fk_orders_customer_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.PhoneNumber)
            .HasDatabaseName("ix_customers_phone_number");

        builder.HasIndex(c => c.UserId)
            .HasDatabaseName("ix_customers_user_id");
    }
}
