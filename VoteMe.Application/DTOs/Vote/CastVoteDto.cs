using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.Vote
{
    public class CastVoteDto
    {
        //[Required]
        //public Guid ElectionId { get; set; }

        //[Required]
        //public Guid ElectionCategoryId { get; set; }

        [Required]
        public Guid CandidateId { get; set; }
    }
}