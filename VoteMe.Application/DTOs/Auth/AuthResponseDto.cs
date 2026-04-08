using System.ComponentModel.DataAnnotations;
using VoteMe.Domain.Enum;

namespace VoteMe.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

    }
}