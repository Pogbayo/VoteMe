using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.OrganizationMember
{
    public class RemoveMemberDto
    {
        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}