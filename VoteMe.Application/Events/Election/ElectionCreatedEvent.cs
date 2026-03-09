namespace VoteMe.Application.Events.Election
{
    public class ElectionCreatedEvent
    {
        public Guid ElectionId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string ElectionName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public List<string> ElectionCategoryNames { get; set; } = new();
        public List<string> MemberEmails { get; set; } = new();
    }
}
