namespace VoteMe.Application.DTOs.Vote
{
    public class VoteResultDto
    {
        public Guid ElectionId { get; set; }
        public Guid ElectionCategoryId { get; set; }
        public Guid CandidateId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public int VoteCount { get; set; }
        public double Percentage { get; set; }
    }
}
