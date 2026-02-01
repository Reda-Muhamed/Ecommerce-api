using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Order
{
    public class OrderDetailsDto
    {
        public Guid OrderId { get; init; }
        public string OrderNumber { get; init; } = null!;
        public string Status { get; init; } = null!;
        public string PaymentStatus { get; init; } = null!;
        public decimal TotalAmount { get; init; }
        public DateTimeOffset CreatedAt { get; init; }

        public IReadOnlyList<OrderItemDto> Items { get; init; } = [];
    }

    public class OrderItemDto
    {
        public string SKU { get; init; } = null!;
        public string Title { get; init; } = null!;
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
        public decimal LineTotal { get; init; }
    }
}
