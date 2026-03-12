namespace VoteMe.Application.Events.Organization
{
    public class MemberLeftOrganizationEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
    }
}
