using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VoteMe.Application.DTOs.Organization
{
    public class UpdateOrganizationDto
    {
        [Required]
        [StringLength(150)]
        public string? Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; } = string.Empty;

        public IFormFile? Logo { get; set; }
    }
}