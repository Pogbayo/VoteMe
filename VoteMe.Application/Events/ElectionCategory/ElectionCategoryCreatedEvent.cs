namespace VoteMe.Application.Events.ElectionCategory
{
    public class ElectionCategoryCreatedEvent
    {
        public Guid ElectionCategoryId { get; set; }
        public string ElectionCategoryName { get; set; } = string.Empty;
        public Guid ElectionId { get; set; }
        public string ElectionName { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
