// Review.cs
using System;
using Ecomm.Core.Entities.User;
namespace Ecomm.Core.Entities.Product
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; } // 1..5
        public string? Title { get; set; }
        public string? Body { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        public Product Product { get; set; } = null!;
        public User.User User { get; set; } = null!;
    }
}
