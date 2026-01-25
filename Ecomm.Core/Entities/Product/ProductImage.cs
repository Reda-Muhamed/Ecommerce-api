using System;

namespace Ecomm.Core.Entities.Product
{
    public class ProductImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public string Url { get; set; } = null!; // cloudinary URL
        public string? PublicId { get; set; } // Cloudinary public_id (for delete/update)
        public bool IsPrimary { get; set; } = false;
        public string? AltText { get; set; }
        public int SortOrder { get; set; } = 0;
        public int? Width { get; set; }
        public int? Height { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation
        public Product Product { get; set; }=null!;
        public ProductVariant? ProductVariant { get; set; }
    }
}
