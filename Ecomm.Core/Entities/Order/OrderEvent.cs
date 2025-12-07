using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Entities.Order
{
    /// <summary>
    /// Order lifecycle events and notes (useful to audit state transitions).
    /// </summary>
    public class OrderEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public string EventType { get; set; } = null!; // e.g., "PaymentCaptured"
        public string? Data { get; set; } // JSON payload details
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public Order Order { get; set; } = null!;
    }
}
