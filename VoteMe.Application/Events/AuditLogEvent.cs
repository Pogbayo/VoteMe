using VoteMe.Domain.Enum;

namespace VoteMe.Application.Events
{
    public class AuditLogEvent
    {
        public Guid UserId { get; set; }
        public AuditAction Action { get; set; }
        public string Entity { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
