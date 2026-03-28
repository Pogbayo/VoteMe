using VoteMe.Application.DTOs.ElectionCategory;

namespace VoteMe.Application.DTOs.Election
{
    public class UpdateElectionDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
    }
}
