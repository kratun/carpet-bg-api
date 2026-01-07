using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;
using CarpetBG.Infrastructure.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.ApplyAuditable();

        builder
            .HasKey(oi => oi.Id)
            .HasName("pk_order_items");

        builder
            .Property(oi => oi.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .Property(oi => oi.Width)
            .HasColumnName("width")
            .HasColumnType("numeric(10,2)");

        builder
            .Property(oi => oi.Height)
            .HasColumnName("height")
            .HasColumnType("numeric(10,2)");

        builder
            .Property(oi => oi.Price)
            .HasColumnName("price")
            .HasColumnType("numeric(12,2)")
            .IsRequired();

        builder
            .Property(oi => oi.Note)
            .HasColumnName("note")
            .HasMaxLength(500)
            .IsRequired();

        builder
            .Property(oi => oi.IsDelivered)
            .HasColumnName("is_delivered")
            .HasDefaultValue(false)
            .IsRequired();

        var ignoreCase = true;
        builder
            .Property(oi => oi.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<OrderItemStatuses>(v, ignoreCase))
            .IsRequired();

        List<string> availableStatuses =
        [
            OrderItemStatuses.New.ToString().ToLowerInvariant(),
            OrderItemStatuses.WashingInProgress.ToString().ToLowerInvariant(),
            OrderItemStatuses.WashingComplete.ToString().ToLowerInvariant(),
        ];

        var statusListSql = string.Join(", ", availableStatuses.Select(s => $"'{s}'"));

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "ck_order_items_status",
                $"status IN ({statusListSql})"
            );
        });

        builder
            .Property(oi => oi.OrderId)
            .HasColumnName("order_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .Property(oi => oi.ProductId)
            .HasColumnName("product_id")
            .HasColumnType("uuid");

        builder
            .HasMany(oi => oi.Additions)
            .WithOne(a => a.OrderItem)
            .HasForeignKey(a => a.OrderItemId)
            .HasConstraintName("fk_additions_order_item_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .HasConstraintName("fk_order_items_order_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .HasConstraintName("fk_order_items_product_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(oi => oi.OrderId)
            .HasDatabaseName("ix_order_items_order_id");

        builder
            .HasIndex(oi => oi.ProductId)
            .HasDatabaseName("ix_order_items_product_id");

        builder
            .HasIndex(oi => oi.Status)
            .HasDatabaseName("ix_order_items_status");
    }
}
