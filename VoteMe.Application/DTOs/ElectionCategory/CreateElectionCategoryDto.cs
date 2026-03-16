using System.ComponentModel.DataAnnotations;
using VoteMe.Application.DTOs.Candidate;

namespace VoteMe.Application.DTOs.ElectionCategory
{
    public class CreateElectionCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "At least one candidate is required.")]
        public List<CreateCandidateDto> Candidates { get; set; } = new();

        public Guid ElectionId { get; set; }
    }
}