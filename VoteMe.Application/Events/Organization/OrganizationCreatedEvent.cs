namespace VoteMe.Application.Events.Organization
{
    public class OrganizationCreatedEvent
    {
        public Guid AdminUserId { get; set; }
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminDisplayName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string UniqueKey { get; set; } = string.Empty;
    }
}