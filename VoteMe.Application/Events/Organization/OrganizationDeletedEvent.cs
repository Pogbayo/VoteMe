namespace VoteMe.Application.Events.Organization
{
    public class OrganizationDeletedEvent
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public Guid DeletedByUserId { get; set; }
        public List<string> MemberEmails { get; set; } = new List<string>();
        public DateTime DeletedAt { get; set; }
    }
}
