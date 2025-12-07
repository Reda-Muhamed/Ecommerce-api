using Ecomm.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Entities.Cart
{
    public class CartItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CartId { get; set; }
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtAddition { get; set; } // snapshot to avoid price volatility
        public DateTimeOffset AddedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation
        public Cart Cart { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
