using VoteMe.Domain.Enum;

namespace VoteMe.Domain.Entities
{
    public class Election : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ElectionStatus Status { get; set; } = ElectionStatus.Pending;
        public bool IsPrivate { get; set; } = false;
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
        public ICollection<ElectionCategory>? Categories { get; set; } = new List<ElectionCategory>();
    }
}
