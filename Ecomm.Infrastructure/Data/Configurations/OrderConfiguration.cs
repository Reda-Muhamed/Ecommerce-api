using Ecomm.Core.Entities;
using Ecomm.Core.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderNumber).IsRequired();
            builder.HasIndex(o => o.OrderNumber).IsUnique();

            builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,4)");

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(o => o.Seller)
                .WithMany()
                .HasForeignKey(o => o.SellerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,4)");
            builder.Property(oi => oi.LineTotal).HasColumnType("decimal(18,4)");
            builder.Property(oi => oi.TaxAmount).HasColumnType("decimal(18,4)");
            builder.Property(oi => oi.DiscountAmount).HasColumnType("decimal(18,4)");

            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
