using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.ElectionCategory
{
    public class UpdateElectionCategoryDto
    {
        [Required]
        public Guid ElectionCategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}