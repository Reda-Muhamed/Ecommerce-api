using Ecomm.Core.Entities.User;
namespace Ecomm.Core.Entities.Cart
{
    public class Cart
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; } // nullable for anonymous (session-based)
        public string? SessionId { get; set; } // when UserId is null
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        public User.User? User { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }

   
}
