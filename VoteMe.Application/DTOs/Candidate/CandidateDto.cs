namespace VoteMe.Application.DTOs.Candidate
{
    public class CandidateDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; } 
        public string PhotoUrl { get; set; } = string.Empty;
        public Guid ElectionCategoryId { get; set; }
    }
}
