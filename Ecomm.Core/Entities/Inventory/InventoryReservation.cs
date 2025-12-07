using Ecomm.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Entities.Inventory
{
    public class InventoryReservation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? OrderId { get; set; } // may be null until order finalized
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset ReservedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ExpiresAt { get; set; }
        public string Status { get; set; } = "Active"; // Active, Released, Fulfilled

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
