// User.cs
using Ecomm.Core.Entities.Product;
using Ecomm.Core.Entities.Order;

namespace Ecomm.Core.Entities.User
{
    /// <summary>
    /// Represents an application user (customers, admins, sellers' accounts).
    /// Password hashing/algorithm metadata stored here (do NOT store raw tokens/passwords).
    /// </summary>
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string _email = null!;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                EmailNormalized = value.ToLowerInvariant();
            }
        }

        public string EmailNormalized { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? FirstName { get; set; }
        public string? SecondName { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        public byte[]? RowVersion { get; set; } // Optional concurrency

        // Navigation
        public Guid RoleId { get; set; } 
        public Role Role { get; set; } = null!;

        public Guid? AddressId { get; set; }
        public Address ?Address { get; set; } = null!;
        public ICollection<Cart.Cart> Carts { get; set; } = new List<Cart.Cart>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Order.Order> Orders { get; set; } = new List<Order.Order>();
    }

}
