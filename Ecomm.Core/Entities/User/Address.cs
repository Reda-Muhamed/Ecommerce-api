// Address.cs
using System;

namespace Ecomm.Core.Entities.User
{
    public class Address
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } // or SellerId for business addresses
        public string Line1 { get; set; } = null!;
        public string? Line2 { get; set; }
        public string City { get; set; } = null!;
        public string? State { get; set; }
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public bool IsDefault { get; set; } = false;
        public string? Type { get; set; } // "billing" / "shipping"

        public User User { get; set; } = null!;
    }
}
