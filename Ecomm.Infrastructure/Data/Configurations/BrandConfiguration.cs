using Ecomm.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brands");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name).IsRequired().HasMaxLength(200);

            builder.Property(b => b.Slug).IsRequired().HasMaxLength(250);

            builder.HasIndex(b => b.Slug)
                .IsUnique()
                .HasDatabaseName("UX_Brands_Slug");

            builder.Property(b => b.Description)
                  .HasMaxLength(1000);

            builder.Property(b => b.IsActive)
                   .HasDefaultValue(true);

            builder.Property(b => b.CreatedAt)
                  .HasDefaultValueSql("sysdatetimeoffset()");

            builder.Property(b => b.UpdatedAt);

            // Concurrency token
            builder.Property(b => b.RowVersion)
                   .IsRowVersion();

             builder.HasQueryFilter(b => !b.IsDeleted);

            // relationships
            builder.HasMany(b => b.Products)
               .WithOne(p => p.Brand)
               .HasForeignKey(p => p.BrandId)
               .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
