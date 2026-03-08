namespace VoteMe.Application.DTOs.Election
{
    public class CreateElectionDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPrivate { get; set; } = false;
        public List<CreateCandidateDto> Candidates { get; set; } = new();
    }
}
