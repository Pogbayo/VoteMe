using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.OrganizationMember
{
    public class AddMemberDto
    {
        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}