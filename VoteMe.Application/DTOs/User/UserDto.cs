using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.User
{
    public class UserDto
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

        public List<string> Roles { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}