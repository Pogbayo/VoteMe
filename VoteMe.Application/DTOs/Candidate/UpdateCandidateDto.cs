using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VoteMe.Application.DTOs.Candidate
{
    public class UpdateCandidateDto
    {
        [StringLength(50)]
        public string? FirstName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? DisplayName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Bio { get; set; } 

        public IFormFile? PhotoFile { get; set; } 
    }
}