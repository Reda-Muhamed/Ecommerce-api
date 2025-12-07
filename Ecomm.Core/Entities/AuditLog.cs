// AuditLog.cs
using System;

namespace Ecomm.Core.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EntityName { get; set; } = null!;
        public string EntityId { get; set; } = null!;
        public string ActionType { get; set; } = null!; // Create/Update/Delete
        public Guid? ActorUserId { get; set; }
        public string? Changes { get; set; } // JSON diff
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public string? CorrelationId { get; set; }
    }
}
