

namespace VoteMe.Application.Events.Election
{
    public class ElectionOpenedEvent
    {
        public Guid ElectionId { get; set; }
        public Guid OrganizationId { get; set; }
        public string ElectionTitle { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public List<string> MemberEmails { get; set; } = new();
    }
}
