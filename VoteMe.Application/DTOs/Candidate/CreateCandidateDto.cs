namespace VoteMe.Application.DTOs.Candidate
{
    public class CreateCandidateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public Guid ElectionCategoryId { get; set; }
    }
}
