using Ecomm.Core.Entities.User;
using System;
using System.Collections.Generic;

namespace Ecomm.Core.Entities.Product
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? SellerId { get; set; } // null => platform-owned
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? BrandId { get; set; }
        public Guid CategoryId { get; set; }

        // Concurrency token
        public byte[]? RowVersion { get; set; }

        // States
        public bool IsActive { get; set; } = true;       // show/hide from customers
        public bool IsPublished { get; set; } = false;   // draft vs published
        public bool IsDeleted { get; set; } = false;     // soft delete
        public DateTimeOffset? DeletedAt { get; set; }   // when deleted

        // Audit
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        // Cached fields
        public decimal? AvgRating { get; set; }
        public int TotalReviews { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public string? SearchKeywords { get; set; }
        public string? CachedPrimaryImageUrl { get; set; }

        // Navigation
        public Seller? Seller { get; set; }
        public Brand? Brand { get; set; }
        public Category Category { get; set; }
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
