using Ecomm.Core.Entities;
using Ecomm.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.SKU)
                   .IsRequired()
                   .HasMaxLength(100);

            // If marketplace: SKU must be unique per product, not globally
            builder.HasIndex(v => new { v.ProductId, v.SKU })
                   .IsUnique()
                   .HasDatabaseName("UX_ProductVariants_Product_SKU");

            // Price precision
            builder.Property(v => v.Price).HasColumnType("decimal(18,4)").IsRequired();
            builder.Property(v => v.CompareAtPrice).HasColumnType("decimal(18,4)");
            builder.Property(v => v.Cost).HasColumnType("decimal(18,4)");

            // Dimensions
            builder.Property(v => v.Weight).HasColumnType("decimal(9,3)");
            builder.Property(v => v.Length).HasColumnType("decimal(9,3)");
            builder.Property(v => v.Width).HasColumnType("decimal(9,3)");
            builder.Property(v => v.Height).HasColumnType("decimal(9,3)");

            // Concurrency
            builder.Property(v => v.RowVersion).IsRowVersion();

            // Timestamps
            builder.Property(v => v.CreatedAt)
                   .HasDefaultValueSql("sysdatetimeoffset()");

            builder.Property(v => v.UpdatedAt);

            // Relationship
            builder.HasOne(v => v.Product)
                   .WithMany(p => p.Variants)
                   .HasForeignKey(v => v.ProductId)
                   .OnDelete(DeleteBehavior.Restrict); // Important: avoid Cascade because Product uses soft-delete

            builder.HasIndex(v => v.ProductId);
        }
    }
}
