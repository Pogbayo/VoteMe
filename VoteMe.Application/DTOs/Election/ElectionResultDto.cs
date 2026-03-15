using VoteMe.Application.DTOs.ElectionCategory;

namespace VoteMe.Application.DTOs.Election
{
    public class ElectionResultDto
    {
        public Guid ElectionId { get; set; }
        public string ElectionName { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public required List<string> WinnersNames { get; set; } = new();
        public List<ElectionCategoryResultDto> CategoryResults { get; set; } = new();
    }
}
