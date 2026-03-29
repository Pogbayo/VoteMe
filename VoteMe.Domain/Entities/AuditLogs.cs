using VoteMe.Domain.Enum;

namespace VoteMe.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public AuditAction Action { get; set; }
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
