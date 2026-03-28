using VoteMe.Domain.Enum;

namespace VoteMe.Domain.Entities
{
    public class OrganizationMember : BaseEntity
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
        public MembershipStatus Status { get; set; } = MembershipStatus.Pending;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; } = false;
    }
}
