using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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

        public IFormFile? PhotoFile { get; set; } 

        [Required]
        public Guid ElectionCategoryId { get; set; }
    }
}