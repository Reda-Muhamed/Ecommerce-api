using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Products
{
    public class GetProductsQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public string? SearchTerm { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? SortBy { get; set; } // price, rating, newest
        public bool SortDesc { get; set; } = true;

    }
}
