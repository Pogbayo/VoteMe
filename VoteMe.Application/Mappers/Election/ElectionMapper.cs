using VoteMe.Application.DTOs.Election;
using VoteMe.Application.DTOs.ElectionCategory;
using VoteMe.Application.Mappers.ElectionCategory;

namespace VoteMe.Application.Mappers.Election
{
    public static class ElectionMapper
    {
        public static ElectionDto ToDto(Domain.Entities.Election election)
        {
            return new ElectionDto
            {
                Id = election.Id,
                Name = election.Name,
                Description = election.Description,
                StartDate = election.StartDate,
                EndDate = election.EndDate,
                Status = election.Status,
                IsPrivate = election.IsPrivate,
                OrganizationId = election.OrganizationId,
                Categories = election.Categories?.Select(ElectionCategoryMapper.ToDto).ToList() ?? new List<ElectionCategoryDto>(),
                CreatedAt = election.CreatedAt
            };
        }

        public static ElectionResultDto ToResultDto(Domain.Entities.Election election, List<ElectionCategoryResultDto> categoryResults)
        {
            return new ElectionResultDto
            {
                ElectionId = election.Id,
                ElectionName = election.Name,
                TotalVotes = categoryResults.Sum(c => c.TotalVotes),
                WinnersNames = categoryResults.Select(c => c.Winner!.DisplayName).ToList()!,
                CategoryResults = categoryResults
            };
        }

        public static IEnumerable<ElectionDto> ToDtoList(IEnumerable<Domain.Entities.Election> elections)
        {
            return elections.Select(ToDto);
        }
    }
}