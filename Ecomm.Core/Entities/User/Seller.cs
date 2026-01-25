// Seller.cs
using System;
using System.Collections.Generic;

namespace Ecomm.Core.Entities.User
{
    public class Seller
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; } // nullable for marketplace-owned items
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        // denormalized / cached
        public decimal? Rating { get; set; }
        public int TotalReviews { get; set; }

        // Navigation
        public User? User { get; set; }
        public ICollection<Product.Product> ?Products { get; set; } = new List<Product.Product>();
    }
}
