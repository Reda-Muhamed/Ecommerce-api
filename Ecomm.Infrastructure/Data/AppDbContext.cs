using Ecomm.Core.Entities;
using Ecomm.Core.Entities.Cart;
using Ecomm.Core.Entities.Inventory;
using Ecomm.Core.Entities.Order;
using Ecomm.Core.Entities.Product;
using Ecomm.Core.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
        public DbSet<VariantAttributeValue> VariantAttributeValues { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Seller> Sellers { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Inventory> Inventories { get; set; } = null!;
        public DbSet<ProductAttribute> ProductAttributes { get; set; } = null!;
        public DbSet<AttributeValue> AttributeValues { get; set; } = null!;
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;
        public DbSet<InventoryReservation> InventoryReservations { get; set; } = null!;
        public DbSet<Wishlist> Wishlists { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           

            // Register all IEntityTypeConfiguration<> in the same assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
