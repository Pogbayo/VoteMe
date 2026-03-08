
namespace VoteMe.Application.DTOs.CreateCandidateDto
{
    public class CandidateResultDto
    {
        public Guid CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public int VoteCount { get; set; }
        public double Percentage { get; set; }
    }
}
