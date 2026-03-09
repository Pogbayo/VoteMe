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
        var winner = results.OrderByDescending(r => r.VoteCount).FirstOrDefault();
        return new ElectionCategoryResultDto
        {
            ElectionCategoryId = category.Id,
            CategoryName = category.Name,
            TotalVotes = results.Sum(r => r.VoteCount),
            FirstName = winner?.FirstName ?? string.Empty,
            LastName = winner?.LastName ?? string.Empty,
            DisplayName = winner?.DisplayName,
            Results = results
        };
    }

    public static IEnumerable<ElectionCategoryDto> ToDtoList(IEnumerable<Domain.Entities.ElectionCategory> categories)
    {
        return categories.Select(ToDto);
    }
}
}
