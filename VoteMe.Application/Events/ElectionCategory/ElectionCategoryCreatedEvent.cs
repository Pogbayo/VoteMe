namespace VoteMe.Application.Events.ElectionCategory
{
    public class ElectionCategoryCreatedEvent
    {
        public Guid CreatedByUserId { get; set; }
        public string ElectionCategoryName { get; set; } = string.Empty;
        public string ElectionName { get; set; } = string.Empty;
    }
}
