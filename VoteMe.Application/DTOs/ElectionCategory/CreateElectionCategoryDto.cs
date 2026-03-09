using VoteMe.Application.DTOs.Candidate;

namespace VoteMe.Application.DTOs.ElectionCategory
{
    public class CreateElectionCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<CreateCandidateDto> Candidates { get; set; } = new();
    }
}
