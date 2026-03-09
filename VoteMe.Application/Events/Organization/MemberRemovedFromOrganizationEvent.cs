

namespace VoteMe.Application.Events.Organization
{
    public class MemberRemovedFromOrganizationEvent
    {
        public Guid RemovedByUserId { get; set; }
        public Guid RemovedUserid { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
    }
}
