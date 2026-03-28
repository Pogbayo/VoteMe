using VoteMe.Domain.Enum;

namespace VoteMe.Application.DTOs.OrganizationMember
{
    public class PendingMemberDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public MembershipStatus Status { get; set; }
    }
}
