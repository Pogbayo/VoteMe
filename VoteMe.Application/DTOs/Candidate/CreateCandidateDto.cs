using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.Candidate
{
    public class CreateCandidateDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? DisplayName { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; } = string.Empty;

        [Url]
        public string? PhotoUrl { get; set; } = string.Empty;

        [Required]
        public Guid ElectionCategoryId { get; set; }
    }
}