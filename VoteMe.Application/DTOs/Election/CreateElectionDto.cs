using VoteMe.Application.DTOs.ElectionCategory;

namespace VoteMe.Application.DTOs.Election
{
    public class CreateElectionDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPrivate { get; set; } = false;
        public Guid OrganizationId { get; set; }
        public List<CreateElectionCategoryDto> Categories { get; set; } = new();
    }
}
