using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.User
{
    public class UpdateUserDto
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(100)]
        public string? DisplayName { get; set; }
    }
}