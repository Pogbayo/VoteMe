using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.Auth
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; } 

        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public required string DisplayName { get; set; }

        [Required]
        public required string UniqueKey { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; } 

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        
    }
}