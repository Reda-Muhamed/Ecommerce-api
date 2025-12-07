// Role.cs (optional simple roles table)
using System;

namespace Ecomm.Core.Entities.User
{
    public class Role
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? UpdatedAt { get; set; }

        // Concurrency token
        public byte[]? RowVersion { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();

    }
}
