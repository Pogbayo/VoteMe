using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.Organization
{
    public class OrganizationDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [EmailAddress]
        public string AdminEmail { get; set; } = string.Empty;

        [Url]
        public string LogoUrl { get; set; } = string.Empty;

        public string UniqueKey { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}