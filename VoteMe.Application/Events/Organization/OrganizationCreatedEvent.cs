namespace VoteMe.Application.Events.Organization
{
    public class OrganizationCreatedEvent
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminFullName { get; set; } = string.Empty;
        public string UniqueKey { get; set; } = string.Empty;
    }
}
