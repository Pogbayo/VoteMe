namespace VoteMe.Application.DTOs.Vote
{
    public class CastVoteDto
    {
        public Guid ElectionId { get; set; }
        public Guid ElectionCategoryId { get; set; }
        public Guid CandidateId { get; set; }
    }
}
