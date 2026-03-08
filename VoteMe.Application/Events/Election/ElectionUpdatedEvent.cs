namespace VoteMe.Application.Events.Election
{

    public class ElectionUpdatedEvent
    {
        public Guid ElectionId { get; set; }
        public Guid OrganizationId { get; set; }
        public string ElectionTitle { get; set; } = string.Empty;
    }
}
