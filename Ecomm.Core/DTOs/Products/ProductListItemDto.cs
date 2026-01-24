using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Products
{
    public class ProductListItemDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public decimal Price { get; init; }

        // MAX 5 preview images
        public IReadOnlyList<string> PreviewImages { get; init; } = [];

        public decimal AverageRating { get; init; }
        public int ReviewsCount { get; init; }
    }


}
