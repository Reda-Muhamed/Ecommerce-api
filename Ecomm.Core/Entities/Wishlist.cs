// Wishlist.cs
using System;

namespace Ecomm.Core.Entities
{
    public class Wishlist
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public DateTimeOffset AddedAt { get; set; } = DateTimeOffset.UtcNow;

        public User.User User { get; set; } = null!;
    }
}
