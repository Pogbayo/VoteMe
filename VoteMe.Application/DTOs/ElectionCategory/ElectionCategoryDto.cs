using System.ComponentModel.DataAnnotations;
using VoteMe.Application.DTOs.Candidate;

namespace VoteMe.Application.DTOs.ElectionCategory
{
    public class ElectionCategoryDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Guid ElectionId { get; set; }

        public List<CandidateDto> Candidates { get; set; } = new();
    }
}