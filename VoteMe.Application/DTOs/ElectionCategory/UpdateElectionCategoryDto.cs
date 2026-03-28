using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.ElectionCategory
{
    public class UpdateElectionCategoryDto
    {
        [Required]

        [StringLength(100)]
        public string? Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; } = string.Empty;
    }
}