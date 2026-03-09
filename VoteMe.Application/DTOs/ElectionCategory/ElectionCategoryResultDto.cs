using VoteMe.Application.DTOs.Candidate;

namespace VoteMe.Application.DTOs.ElectionCategory
{
    public class ElectionCategoryResultDto
    {
        public Guid ElectionCategoryId { get; set; }
        public string ElectionCategoryName { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public WinnerDto? Winner { get; set; }
        public List<CandidateResultDto> Results { get; set; } = new();
    }
}
