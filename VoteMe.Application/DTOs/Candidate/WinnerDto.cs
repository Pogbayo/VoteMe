using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.Candidate
{
    public class WinnerDto
    {
        [Required]
        public Guid CandidateId { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? DisplayName { get; set; }

        [Range(0, int.MaxValue)]
        public int VoteCount { get; set; }

        [Range(0, 100)]
        public double Percentage { get; set; }
        public bool IsTie { get; set; }        
        public List<TiedCandidateDto>? TiedCandidates { get; set; } 
    }
}