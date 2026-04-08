using System.ComponentModel.DataAnnotations;
using VoteMe.Domain.Enum;

namespace VoteMe.Application.DTOs.User
{
    public class OrganizationUserDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public OrganizationRole Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
