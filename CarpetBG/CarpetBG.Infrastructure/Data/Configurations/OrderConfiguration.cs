using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;
using CarpetBG.Infrastructure.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders", t =>
        {
            // CHECK constraint for string enum
            var statuses = Enum
                .GetNames<OrderStatuses>()
                .Select(s => s.ToLowerInvariant());

            var statusSql = string.Join(", ", statuses.Select(s => $"'{s}'"));

            t.HasCheckConstraint(
                "ck_orders_status",
                $"status IN ({statusSql})"
            );
        });

        builder.ApplyAuditable();

        builder.HasKey(o => o.Id)
               .HasName("pk_orders");

        builder.Property(o => o.Id)
               .HasColumnName("id")
               .HasColumnType("uuid")
               .IsRequired();

        builder.Property(o => o.PickupAddressId)
               .HasColumnName("pickup_address_id")
               .HasColumnType("uuid")
               .IsRequired();

        builder.Property(o => o.PickupTimeRange)
               .HasColumnName("pickup_time_range")
               .HasMaxLength(100);

        builder.Property(o => o.PickupDate)
               .HasColumnName("pickup_date")
               .HasColumnType("date");

        builder.Property(o => o.DeliveryAddressId)
               .HasColumnName("delivery_address_id")
               .HasColumnType("uuid");

        builder.Property(o => o.DeliveryTimeRange)
               .HasColumnName("delivery_time_range")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(o => o.DeliveryDate)
               .HasColumnName("delivery_date")
               .HasColumnType("date");

        builder.Property(o => o.OrderBy)
               .HasColumnName("order_by");

        builder.Property(o => o.CustomerId)
               .HasColumnName("customer_id")
               .HasColumnType("uuid")
               .IsRequired();

        builder.Property(o => o.Status)
               .HasColumnName("status")
               .HasMaxLength(50)
               .HasConversion(
                   v => v.ToString().ToLowerInvariant(),
                   v => Enum.Parse<OrderStatuses>(v, true)
               )
               .IsRequired();

        builder.Property(o => o.Note)
               .HasColumnName("note")
               .HasMaxLength(1000)
               .IsRequired();

        builder.HasOne(o => o.Customer)
               .WithMany(c => c.Orders)
               .HasForeignKey(o => o.CustomerId)
               .HasConstraintName("fk_orders_customer_id")
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.PickupAddress)
               .WithMany()
               .HasForeignKey(o => o.PickupAddressId)
               .HasConstraintName("fk_orders_pickup_address_id")
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.DeliveryAddress)
               .WithMany()
               .HasForeignKey(o => o.DeliveryAddressId)
               .HasConstraintName("fk_orders_delivery_address_id")
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
               .WithOne(oi => oi.Order)
               .HasForeignKey(oi => oi.OrderId)
               .HasConstraintName("fk_order_items_order_id")
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.CustomerId)
               .HasDatabaseName("ix_orders_customer_id");

        builder.HasIndex(o => o.Status)
               .HasDatabaseName("ix_orders_status");
    }
}
