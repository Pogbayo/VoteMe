using VoteMe.Domain.Enum;

namespace VoteMe.Application.DTOs.OrganizationMember
{
    public class OrganizationMemberDto
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set;}
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public OrganizationRole Role { get; set; }
        public MembershipStatus Status { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
