using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.OrganizationMember
{
    public class UpdateRoleDto
    {
        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public bool IsAdmin { get; set; }
    }
}