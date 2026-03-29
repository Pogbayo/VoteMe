using System.ComponentModel.DataAnnotations;

namespace VoteMe.Application.DTOs.Election
{
    public class OpenElectionDto
    {
        [Required]
        public DateTime EndDate { get; set; }
    }
}
