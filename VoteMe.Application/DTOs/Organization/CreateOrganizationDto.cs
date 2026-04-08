using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VoteMe.Application.DTOs.Organization
{
    public class CreateOrganizationDto
    {
        [Required]
        [StringLength(150)]
        public string OrganizationName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? DisplayName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MinLength(6)]
        public string? Password { get; set; }

        public IFormFile? LogoFile { get; set; }
    }
}