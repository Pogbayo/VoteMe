namespace VoteMe.Application.Events.ElectionCategory
{
    public class ElectionCategoryDeletedEvent
    {
        public Guid DeletedByUserId { get; set; }
        public string ElectionCategoryName { get; set; } = string.Empty;
        public string ElectionName { get; set; } = string.Empty;
    }
}
