using Ecomm.Core.Entities;
using Ecomm.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImages");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Url).IsRequired();

            builder.HasOne(i => i.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.ProductVariant)
                .WithMany(v => v.Images)
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
