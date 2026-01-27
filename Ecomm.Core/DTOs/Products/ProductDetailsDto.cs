using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Products
{
    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string ?Description { get; set; } = null!;

        public string? Brand { get; set; } = null!;
        public string? Category { get; set; } = null!;

        public decimal PriceFrom { get; set; }
        public decimal PriceTo { get; set; }

        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }

        public IReadOnlyList<ProductImageDto> Images { get; set; } = [];
        public IReadOnlyList<ProductVariantDto> Variants { get; set; } = [];
    }
    public class ProductImageVariantDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = null!;
       
    }

}
