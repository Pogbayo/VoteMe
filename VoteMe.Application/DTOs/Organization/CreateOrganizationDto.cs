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
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AdminFirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AdminLastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string AdminDisplayName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string AdminEmail { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string AdminPhoneNumber { get; set; } = string.Empty;

        public IFormFile? LogoFile { get; set; } 


        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}