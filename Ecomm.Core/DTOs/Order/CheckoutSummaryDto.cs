using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Order
{
    public class CheckoutSummaryDto
    {
        public List<CheckoutItemDto> Items { get; init; } = [];
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CheckoutItemDto
    {
        public Guid ProductId { get; init; }
        public Guid VariantId { get; init; }
        public string SKU { get; init; } = null!;
        public string Title { get; init; } = null!;
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
        public decimal LineTotal { get; init; }
    }
}
