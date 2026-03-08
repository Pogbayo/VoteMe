namespace VoteMe.Application.DTOs.Vote
{
    public class VoteResultDto
    {
        public Guid ElectionId { get; set; }
        public Guid CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public int VoteCount { get; set; }
        public double Percentage { get; set; }
    }
}
