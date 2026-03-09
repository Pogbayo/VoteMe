namespace VoteMe.Application.Events.Election
{

    public class ElectionUpdatedEvent
    {
        public Guid ElectionId { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public string ElectionName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
    }
}
