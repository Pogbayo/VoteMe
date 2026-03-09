namespace VoteMe.Application.Events.ElectionCategory
{
    public class ElectionCategoryUpdatedEvent
    {
        public Guid UpdatedByUserId { get; set; }
        public string ElectionCategoryName { get; set; } = string.Empty;
        public string ElectionName { get; set; } = string.Empty;
    }
}
