using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.Vote
{
    public class VoteResultDto
    {
        public Guid ElectionId { get; set; }

        public Guid ElectionCategoryId { get; set; }

        public Guid CandidateId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? DisplayName { get; set; }

        [Range(0, int.MaxValue)]
        public int VoteCount { get; set; }

        [Range(0, 100)]
        public double Percentage { get; set; }
    }
}