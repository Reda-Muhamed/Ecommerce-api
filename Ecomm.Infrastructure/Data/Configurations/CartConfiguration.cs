using Ecomm.Core.Entities;
using Ecomm.Core.Entities.Cart;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");
            builder.HasKey(c => c.Id);

            // allow nullable UserId for guest carts
            builder.Property(c => c.UserId).IsRequired(false);

            // SessionId for guest carts
            builder.Property(c => c.SessionId).HasMaxLength(200);

            builder.Property(c => c.CreatedAt).HasDefaultValueSql("sysdatetimeoffset()");
            builder.Property(c => c.UpdatedAt);

            // Indexes:
            builder.HasIndex(c => c.UserId).HasDatabaseName("IX_Carts_UserId");
            builder.HasIndex(c => c.SessionId).HasDatabaseName("IX_Carts_SessionId");

            // Enforce at-most-one active cart per logged-in user (filtered unique index)
            // SQL Server example: ignore NULL UserId (guests) so many guest carts allowed.
            builder.HasIndex(c => c.UserId)
                   .IsUnique()
                   .HasFilter("[UserId] IS NOT NULL")
                   .HasDatabaseName("UX_Carts_User_OneActive");

            // Enforce at-most-one cart per session for guests
            builder.HasIndex(c => c.SessionId)
                   .IsUnique()
                   .HasFilter("[SessionId] IS NOT NULL")
                   .HasDatabaseName("UX_Carts_Session_One");

            builder.HasOne(c => c.User)
                   .WithMany(u => u.Carts)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");
            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.PriceAtAddition).HasColumnType("decimal(18,4)");
            builder.Property(ci => ci.AddedAt).HasDefaultValueSql("sysdatetimeoffset()");

            // Relationships
            builder.HasOne(ci => ci.Cart)
                   .WithMany(c => c.Items)
                   .HasForeignKey(ci => ci.CartId)
                   .OnDelete(DeleteBehavior.Cascade); // delete items when cart deleted

            builder.HasOne(ci => ci.ProductVariant)
                   .WithMany() // WithMany(v => v.CartItems) if want navigation back
                   .HasForeignKey(ci => ci.ProductVariantId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes: fast lookups & avoid duplicates within same cart
            builder.HasIndex(ci => ci.CartId).HasDatabaseName("IX_CartItems_CartId");
            builder.HasIndex(ci => ci.ProductVariantId).HasDatabaseName("IX_CartItems_VariantId");

            
        }
    }
}
