using VoteMe.Application.DTOs.ElectionCategory;

namespace VoteMe.Application.DTOs.Election
{
    public class UpdateElectionDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPrivate { get; set; }
        public List<UpdateElectionCategoryDto> Categories { get; set; } = new();
    }
}
