

namespace VoteMe.Application.Events.Organization
{
    public class MemberRemovedFromOrganizationEvent
    {
        public Guid RemovedByUserId { get; set; }
        public Guid RemovedUserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
    }
}
