using Ecomm.Core.Entities.User;
namespace Ecomm.Core.Entities.Order
{
    /// <summary>
    /// Order aggregate root. Contains snapshot order lines and events.
    /// </summary>
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OrderNumber { get; set; } = null!; // e.g., "ORD-20251212-0001"
        public Guid? UserId { get; set; }
        public Guid? SellerId { get; set; } // optional per-seller split
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // use enum-like strings or enum in domain
        public string PaymentStatus { get; set; } = "Pending";
        public Guid? ShippingAddressId { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        // Navigation
        public User.User? User { get; set; }
        public Seller? Seller { get; set; }
        public Address? ShippingAddress { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
        public ICollection<OrderEvent> Events { get; set; } = new List<OrderEvent>();
    }
}
