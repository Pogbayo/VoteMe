namespace VoteMe.Domain.Entities
{
    public class Candidate : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Bio { get; set; } 
        public string PhotoUrl { get; set; } = string.Empty;
        //public Election Election { get; set; } = null!;
        public Guid ElectionCategoryId { get; set; }
        public ElectionCategory ElectionCategory { get; set; } = null!;
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
