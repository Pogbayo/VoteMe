using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.Candidate
{
    public class CandidateResultDto
    {
        [Required]
        public Guid CandidateId { get; set; }

        [Required]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int VoteCount { get; set; }

        [Range(0, 100)]
        public double Percentage { get; set; }
    }
}