using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Products
{
    public class CreateProductDto
    {
        public string Name { get; set; }= null!;
        public string ?Description { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? BrandId { get; set; }

    }
    public class CreateVariantDto
    {
        public string SKU { get; set; } = null!;

        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }

        public int StockQuantity { get; set; }

        // Attributes like Color, Size
        public IReadOnlyList<Guid> AttributeValueIds { get; set; } = [];
    }
    public class AddVariantImagesDto
    {
        public List<IFormFile> ImageUrls { get; set; } = new();
    }


}
