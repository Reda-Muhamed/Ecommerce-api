using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Products
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public string SKU { get; set; } = null!;
        public string? Title { get; set; }
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }

        public bool IsInStock { get; set; }

        public IReadOnlyList<VariantAttributeDto> Attributes { get; set; } = [];
        public IReadOnlyList<ProductImageVariantDto> Images { get; set; } = [];
    }


}
