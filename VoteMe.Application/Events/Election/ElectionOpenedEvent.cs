

namespace VoteMe.Application.Events.Election
{
    public class ElectionOpenedEvent
    {
        public Guid ElectionId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid OpenedByUserId { get; set; }
        public string ElectionName { get; set; } = string.Empty;
        public List<string> ElectionCategoryNames { get; set; } = new List<string>();
        public string OrganizationName { get; set; } = string.Empty;
        public List<string> MemberEmails { get; set; } = new();
    }
}
