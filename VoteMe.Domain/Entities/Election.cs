
using VoteMe.Domain.Enum;

namespace VoteMe.Domain.Entities
{
    public class Election : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ElectionStatus Status { get; set; } = ElectionStatus.Pending;
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
        public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public bool IsPrivate { get; set; } = false;
    }
}
