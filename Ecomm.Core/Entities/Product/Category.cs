// Category.cs
using System;
using System.Collections.Generic;

namespace Ecomm.Core.Entities.Product
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public Guid? ParentCategoryId { get; set; }
        public int SortOrder { get; set; } = 0;//Priority
        public bool IsActive { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        public byte[]? RowVersion { get; set; } // Concurrency token

        // Navigation
        public Category? ParentCategory { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
