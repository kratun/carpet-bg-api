using CarpetBG.Domain.Entities;
using CarpetBG.Infrastructure.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.ApplyAuditable();

        builder.HasKey(p => p.Id)
               .HasName("pk_products");

        builder.Property(p => p.Id)
               .HasColumnName("id")
               .HasColumnType("uuid")
               .IsRequired();

        builder.Property(p => p.Price)
               .HasColumnName("price")
               .HasColumnType("numeric(12,2)")
               .IsRequired();

        builder.Property(p => p.ExpressServicePrice)
               .HasColumnName("express_service_price")
               .HasColumnType("numeric(12,2)")
               .IsRequired();

        builder.Property(p => p.Name)
               .HasColumnName("name")
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(p => p.NormalizedName)
               .HasColumnName("normalized_name")
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(p => p.Description)
               .HasColumnName("description")
               .HasMaxLength(2000)
               .IsRequired();

        builder.Property(p => p.OrderBy)
               .HasColumnName("order_by")
               .IsRequired();

        builder.HasMany(p => p.OrderItems)
               .WithOne(oi => oi.Product)
               .HasForeignKey(oi => oi.ProductId)
               .HasConstraintName("fk_order_items_product_id")
               .OnDelete(DeleteBehavior.Restrict);

        // Soft-delete aware unique index
        builder.HasIndex(p => p.NormalizedName)
               .HasDatabaseName("ux_products_normalized_name_active")
               .IsUnique()
               .HasFilter("is_deleted = false");

        // Index for ordering / sorting
        builder.HasIndex(p => p.OrderBy)
               .HasDatabaseName("ix_products_order_by");
    }
}
