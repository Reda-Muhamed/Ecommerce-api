// PaymentTransaction.cs
using System;

namespace Ecomm.Core.Entities
{
    public class PaymentTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public string TransactionId { get; set; } = null!; // provider id
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!; // e.g., "Success","Failed"
        public string Method { get; set; } = null!; // e.g., "Card","PayPal"
        public string? ProviderResponse { get; set; } // raw or JSON digest
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public Order.Order Order { get; set; } = null!;
    }
}
