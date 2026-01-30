using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Cart
{
    public class CartDto
    {
        public Guid CartId { get; set; }
        public IReadOnlyCollection<CartItemDto> Items { get; set; } = [];
        public decimal TotalPrice { get; set; }
    }
    public class CartItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductVariantId { get; set; }
        public string Sku { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
    }

}
