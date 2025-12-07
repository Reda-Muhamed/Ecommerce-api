using Ecomm.Core.Entities;
using Ecomm.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class VariantAttributeValueConfiguration : IEntityTypeConfiguration<VariantAttributeValue>
    {
        public void Configure(EntityTypeBuilder<VariantAttributeValue> builder)
        {
            builder.ToTable("VariantAttributeValues");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.ProductVariant)
                .WithMany(v => v.VariantAttributeValues)
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Attribute)
                .WithMany()
                .HasForeignKey(x => x.AttributeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AttributeValue)
                .WithMany()
                .HasForeignKey(x => x.AttributeValueId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ProductVariantId, x.AttributeId }).IsUnique();

            // Index to help filter by attribute value e.g., find variants that have Color=Red
            builder.HasIndex(x => new { x.AttributeId, x.AttributeValueId });
        }
    }
}
