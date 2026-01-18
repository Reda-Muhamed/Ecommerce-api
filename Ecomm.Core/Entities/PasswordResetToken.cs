using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Entities
{
    public class PasswordResetToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = null!;
        public string Purpose { get; set; } = "Reset Passsword";

        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public bool IsUsed { get; set; }
        public DateTimeOffset? UsedAt { get; set; }
        public User.User User { get; set; } = null!;
    }
}

