using VoteMe.Application.DTOs.Candidate;
using VoteMe.Application.DTOs.ElectionCategory;
using VoteMe.Application.Mappers.Candidate;

namespace VoteMe.Application.Mappers.ElectionCategory
{
    public static class ElectionCategoryMapper
    {
        public static ElectionCategoryDto ToDto(Domain.Entities.ElectionCategory category)
        {
            return new ElectionCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ElectionId = category.ElectionId,
                Candidates = category.Candidates?.Select(CandidateMapper.ToDto).ToList() ?? new List<CandidateDto>()
            };
        }

        public static ElectionCategoryResultDto ToResultDto(Domain.Entities.ElectionCategory category, List<CandidateResultDto> results)
        {
            if (!results.Any())
            {
                return new ElectionCategoryResultDto
                {
                    ElectionCategoryId = category.Id,
                    ElectionCategoryName = category.Name,
                    TotalVotes = 0,
                    Winner = null,
                    Results = results
                };
            }

            var highestVotes = results.Max(r => r.VoteCount);
            var topCandidates = results
                .Where(r => r.VoteCount == highestVotes)
                .ToList();

            var isTie = topCandidates.Count > 1;
            var winner = topCandidates.First(); // only used when isTie = false

            return new ElectionCategoryResultDto
            {
                ElectionCategoryId = category.Id,
                ElectionCategoryName = category.Name,
                TotalVotes = results.Sum(r => r.VoteCount),
                Winner = winner == null ? null : new WinnerDto
                {
                    CandidateId = winner.CandidateId,
                    FirstName = winner.FirstName,
                    LastName = winner.LastName,
                    DisplayName = winner.DisplayName,
                    VoteCount = winner.VoteCount,
                    Percentage = winner.Percentage,
                    IsTie = isTie,
                    TiedCandidates = isTie ? topCandidates.Select(c => new TiedCandidateDto
                    {
                        CandidateId = c.CandidateId,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        DisplayName = c.DisplayName,
                        VoteCount = c.VoteCount,
                        Percentage = c.Percentage
                    }).ToList() : null
                },
                Results = results
            };
        }

        public static IEnumerable<ElectionCategoryDto> ToDtoList(IEnumerable<Domain.Entities.ElectionCategory> categories)
    {
        return categories.Select(ToDto);
    }
}
}
