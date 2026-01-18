using Ecomm.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.ToTable("Wishlists");
            builder.HasKey(w => w.Id);

            builder.HasOne(w => w.User)
            .WithMany()
            .HasForeignKey(w => w.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);


            builder.HasIndex(w => new { w.UserId, w.ProductVariantId }).IsUnique(false);
        }
    }
}
