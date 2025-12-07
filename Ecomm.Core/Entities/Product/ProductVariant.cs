// ProductVariant.cs
using System;
using System.Collections.Generic;
using Ecomm.Core.Entities.Order;
using Ecomm.Core.Entities.Inventory;

namespace Ecomm.Core.Entities.Product
{
    public class ProductVariant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public string SKU { get; set; } = null!;
        public string? Barcode { get; set; }
        public string? Title { get; set; } // human-friendly "T-Shirt / Red / L"
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Weight { get; set; } // normalized unit (e.g., kg)
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        public int StockQuantity { get; set; }
        public bool IsBackorderAllowed { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        public byte[]? RowVersion { get; set; }

        // Navigation
        public Product Product { get; set; } = null!;
        public ICollection<VariantAttributeValue> VariantAttributeValues { get; set; } = new List<VariantAttributeValue>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Inventory.Inventory> Inventories { get; set; } = new List<Inventory.Inventory>();
    }
}
