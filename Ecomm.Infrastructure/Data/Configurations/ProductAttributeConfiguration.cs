using Ecomm.Core.Entities;
using Ecomm.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable("ProductAttributes");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(a => a.Type)
                   .HasMaxLength(50);

            builder.Property(a => a.IsFilterable)
                   .HasDefaultValue(false);

            builder.Property(a => a.IsVariantable)
                   .HasDefaultValue(false);

           

            // index on name for admin lookup
            builder.HasIndex(a => a.Name)
                   .HasDatabaseName("IX_ProductAttributes_Name");
        }
    }

    public class AttributeValueConfiguration : IEntityTypeConfiguration<AttributeValue>
    {
        public void Configure(EntityTypeBuilder<AttributeValue> builder)
        {
            builder.ToTable("AttributeValues");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Value)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(v => v.SortOrder)
                   .HasDefaultValue(0);

            // Relationship: AttributeValue -> ProductAttribute (many-to-one)
            builder.HasOne(v => v.Attribute)
                   .WithMany(a => a.Values)
                   .HasForeignKey(v => v.AttributeId)
                   .OnDelete(DeleteBehavior.Cascade); // OK: deleting attribute removes values

            // Make (AttributeId, Value) unique to avoid duplicates for same attribute
            builder.HasIndex(v => new { v.AttributeId, v.Value })
                   .IsUnique()
                   .HasDatabaseName("UX_AttributeValues_Attribute_Value");

            // helpful index for queries by AttributeId
            builder.HasIndex(v => v.AttributeId)
                   .HasDatabaseName("IX_AttributeValues_AttributeId");

            
        }
    }
}
