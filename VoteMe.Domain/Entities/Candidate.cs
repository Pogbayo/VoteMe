namespace VoteMe.Domain.Entities
{
    public class Candidate : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public Guid ElectionId { get; set; }
        public Election Election { get; set; } = null!;
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
