using VoteMe.Application.DTOs.Candidate;
using VoteMe.Domain.Entities;

namespace VoteMe.Application.Mappers.Candidate
{
    // Mappers/Candidate/CandidateMapper.cs
    public static class CandidateMapper
    {
        public static CandidateDto ToDto(Domain.Entities.Candidate candidate)
        {
            return new CandidateDto
            {
                Id = candidate.Id,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                DisplayName = candidate.DisplayName!,
                Bio = candidate.Bio,
                PhotoUrl = candidate.PhotoUrl,
                ElectionCategoryId = candidate.ElectionCategoryId
            };
        }

        public static CandidateResultDto ToResultDto(Domain.Entities.Candidate candidate, int voteCount, int totalVotes)
        {
            return new CandidateResultDto
            {
                CandidateId = candidate.Id,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                DisplayName = candidate.DisplayName!,
                VoteCount = voteCount,
                Percentage = totalVotes == 0 ? 0 : Math.Round((double)voteCount / totalVotes * 100, 2)
            };
        }

        public static IEnumerable<CandidateDto> ToDtoList(IEnumerable<Domain.Entities.Candidate> candidates)
        {
            return candidates.Select(ToDto);
        }
    }
}