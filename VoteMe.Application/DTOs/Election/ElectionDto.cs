using VoteMe.Application.DTOs.CreateCandidateDto;

namespace VoteMe.Application.DTOs.Election
{
    public class ElectionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public Guid OrganizationId { get; set; }
        public List<CandidateDto> Candidates { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
