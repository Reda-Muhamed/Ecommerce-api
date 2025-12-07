using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Entities.Order
{
    /// <summary>
    /// Immutable order line snapshot — copies SKU, title and price at purchase time.
    /// </summary>
    public class OrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; } // snapshot pointer
        public Guid? ProductVariantId { get; set; } // snapshot pointer (may be null)
        public string SKU { get; set; } = null!;
        public string? Title { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal LineTotal { get; set; } // stored immutably

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
    }
}
