namespace VoteMe.Domain.Entities
{
    public class ElectionCategory : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ElectionId { get; set; }
        public Election Election { get; set; } = null!;
        public ICollection<Candidate>? Candidates { get; set; } = new List<Candidate>();
    }
}
