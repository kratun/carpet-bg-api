using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;
using CarpetBG.Infrastructure.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarpetBG.Infrastructure.Data.Configurations;

public class AdditionConfiguration : IEntityTypeConfiguration<Addition>
{
    public void Configure(EntityTypeBuilder<Addition> builder)
    {
        builder.ToTable("additions", t =>
        {
            // CHECK constraint for string enum
            var unspecified = AdditionTypes.Unspecified.ToString();
            var additionTypes = Enum
                .GetNames<AdditionTypes>()
                .Where(s => s != unspecified)
                .Select(s => s.ToLowerInvariant());

            var additionTypesSql = string.Join(", ", additionTypes.Select(s => $"'{s}'"));

            t.HasCheckConstraint(
                "ck_additions_type",
                $"addition_type IN ({additionTypesSql})"
            );
        });

        builder.ApplyAuditable();

        builder.HasKey(a => a.Id)
               .HasName("pk_additions");

        builder.Property(a => a.Id)
               .HasColumnName("id")
               .HasColumnType("uuid")
               .IsRequired();

        builder.Property(a => a.Name)
               .HasColumnName("name")
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(a => a.NormalizedName)
               .HasColumnName("normalized_name")
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(a => a.AdditionType)
               .HasColumnName("addition_type")
               .HasMaxLength(50)
               .HasConversion(
                   v => v.ToString().ToLowerInvariant(),
                   v => Enum.Parse<AdditionTypes>(v, true)
               )
               .IsRequired();

        builder.Property(a => a.Value)
               .HasColumnName("value")
               .HasColumnType("numeric(12,2)")
               .IsRequired();

        builder.Property(a => a.OrderItemId)
               .HasColumnName("order_item_id")
               .HasColumnType("uuid")
               .IsRequired();

        builder.HasOne(a => a.OrderItem)
               .WithMany(oi => oi.Additions)
               .HasForeignKey(a => a.OrderItemId)
               .HasConstraintName("fk_additions_order_item_id")
               .OnDelete(DeleteBehavior.Restrict);
    }
}
