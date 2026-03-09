
namespace VoteMe.Application.DTOs.Candidate
{
    public class CandidateResultDto
    {
        public Guid CandidateId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int VoteCount { get; set; }
        public double Percentage { get; set; }
    }
}
