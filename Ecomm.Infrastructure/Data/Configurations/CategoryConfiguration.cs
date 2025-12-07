using Ecomm.Core.Entities;
using Ecomm.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(c => c.Slug)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.HasIndex(c => c.Slug)
                   .IsUnique()
                   .HasDatabaseName("UX_Categories_Slug");

            
            // timestamps & concurrency
            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("sysdatetimeoffset()")
                   .IsRequired();

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.RowVersion)
                   .IsRowVersion();

            //useful to hide categories without deleting
            builder.Property(c => c.IsActive)
                   .HasDefaultValue(true);


            // Relationship (self-reference)
            builder.HasOne(c => c.ParentCategory)
                   .WithMany(p => p.Children)
                   .HasForeignKey(c => c.ParentCategoryId)
                   .OnDelete(DeleteBehavior.Restrict); // avoid cascade delete down the tree

            builder.HasQueryFilter(c => c.IsActive);

            
        }
    }
}
