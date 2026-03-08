

namespace VoteMe.Application.Events.Organization
{
    public class MemberRemovedFromOrganizationEvent
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
    }
}
