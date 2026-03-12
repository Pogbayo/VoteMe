using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.Vote
{
    public class ChangeVoteDto
    {
        [Required]
        public Guid ElectionId { get; set; }

        [Required]
        public Guid ElectionCategoryId { get; set; }

        [Required]
        public Guid NewCandidateId { get; set; }
    }
}