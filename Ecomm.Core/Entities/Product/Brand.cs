// Brand.cs
using System;
using System.Collections.Generic;

namespace Ecomm.Core.Entities.Product
{
    public class Brand
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public byte[]? RowVersion { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
