// RefreshToken.cs
using System;

namespace Ecomm.Core.Entities
{
    /// <summary>
    /// Refresh tokens stored as hashed values (store only hash in DB).
    /// </summary>
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = null!;
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? CreatedByIp { get; set; }
        public DateTimeOffset? RevokedAt { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public bool IsActive => RevokedAt == null && DateTimeOffset.UtcNow <= ExpiresAt;

        // Navigation
        public User.User User { get; set; } = null!;
    }
}
