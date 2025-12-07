using Ecomm.Core.Entities.Product;

namespace Ecomm.Core.Entities.Inventory
{
    public class Inventory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductVariantId { get; set; }

        public string Location { get; set; } = null!; // e.g., "Warehouse A", "Store #5"
        public int AvailableQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;
    }

    
}
