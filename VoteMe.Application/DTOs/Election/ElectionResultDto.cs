using VoteMe.Application.DTOs.Candidate;

namespace VoteMe.Application.DTOs.Election
{
    public class ElectionResultDto
    {
        public Guid ElectionId { get; set; }
        public string ElectionTitle { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public string WinnerName { get; set; } = string.Empty;
        public List<CandidateResultDto> Results { get; set; } = new();
    }
}
