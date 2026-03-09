using VoteMe.Application.DTOs.Vote;

namespace VoteMe.Application.Mappers.Vote
{
    public static class VoteMapper
    {
        public static VoteResultDto ToResultDto(Domain.Entities.Vote vote, int voteCount, int totalVotes)
        {
            return new VoteResultDto
            {
                ElectionId = vote.ElectionId,
                ElectionCategoryId = vote.ElectionCategoryId,
                CandidateId = vote.CandidateId,
                FirstName = vote.Candidate.FirstName,
                LastName = vote.Candidate.LastName,
                DisplayName = vote.Candidate.DisplayName,
                VoteCount = voteCount,
                Percentage = totalVotes == 0 ? 0 : Math.Round((double)voteCount / totalVotes * 100, 2)
            };
        }
    }
}