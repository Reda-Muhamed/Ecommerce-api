using Ecomm.Core.Entities;
using Ecomm.Core.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecomm.Infrastructure.Data.Configurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable("Inventories");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.AvailableQuantity).IsRequired();
            builder.Property(i => i.ReservedQuantity).IsRequired();

            builder.HasOne(i => i.ProductVariant)
                .WithMany(v => v.Inventories)
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

    public class InventoryReservationConfiguration : IEntityTypeConfiguration<InventoryReservation>
    {
        public void Configure(EntityTypeBuilder<InventoryReservation> builder)
        {
            builder.ToTable("InventoryReservations");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Quantity).IsRequired();

            builder.HasOne(r => r.ProductVariant)
                .WithMany()
                .HasForeignKey(r => r.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.OrderId);
        }
    }
}
