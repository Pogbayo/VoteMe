

namespace VoteMe.Domain.Entities
{
    public class Vote : BaseEntity
    {
        public Guid CandidateId { get; set; }
        public Candidate Candidate { get; set; } = null!;
        public Guid ElectionId { get; set; }
        public Election Election { get; set; } = null!;
        public Guid VoterId { get; set; } 
        public AppUser Voter { get; set; } = null!; 
    }
}
