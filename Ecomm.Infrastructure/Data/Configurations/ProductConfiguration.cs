using Ecomm.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(p => p.Slug)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(p => p.Description)
                   .HasMaxLength(4000);

            // Soft-delete and audit
            builder.Property(p => p.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(p => p.DeletedAt);

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("sysdatetimeoffset()")
                   .IsRequired();

            builder.Property(p => p.UpdatedAt);

            // Concurrency token
            builder.Property(p => p.RowVersion)
                   .IsRowVersion();

            // Cached numeric fields
            builder.Property(p => p.PriceMin).HasColumnType("decimal(18,4)");
            builder.Property(p => p.PriceMax).HasColumnType("decimal(18,4)");
            builder.Property(p => p.AvgRating).HasColumnType("decimal(3,2)");

            // Indexes
            builder.HasIndex(p => p.SellerId).HasDatabaseName("IX_Products_SellerId");
            builder.HasIndex(p => p.CategoryId).HasDatabaseName("IX_Products_CategoryId");
            builder.HasIndex(p => p.BrandId).HasDatabaseName("IX_Products_BrandId");

            builder.HasIndex(p => new { p.SellerId, p.Slug })
                   .IsUnique()
                   .HasDatabaseName("UX_Products_Seller_Slug")
                   .HasFilter("[IsDeleted] = 0");

            builder.HasQueryFilter(p => !p.IsDeleted);

            // Relationships 
            // Seller (nullable) - keep product when seller removed
            builder.HasOne(p => p.Seller)
                   .WithMany(s => s.Products)
                   .HasForeignKey(p => p.SellerId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Brand (nullable) - set null on brand delete
            builder.HasOne(p => p.Brand)
                   .WithMany(b => b.Products)
                   .HasForeignKey(p => p.BrandId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Category (nullable) - set null on category delete
            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
