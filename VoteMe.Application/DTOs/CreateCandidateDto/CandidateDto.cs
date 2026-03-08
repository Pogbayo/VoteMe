namespace VoteMe.Application.DTOs.CreateCandidateDto
{
    public class CandidateDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public Guid ElectionId { get; set; }
    }
}
