
namespace VoteMe.Application.DTOs.Vote
{
    public class ChangeVoteDto
    {
        public Guid ElectionId { get; set; }
        public Guid NewCandidateId { get; set; }
    }
}
