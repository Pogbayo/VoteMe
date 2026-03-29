using VoteMe.Application.DTOs.Candidate;
using VoteMe.Application.DTOs.ElectionCategory;
using VoteMe.Domain.Enum;

namespace VoteMe.Application.DTOs.Election
{
    public class ElectionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ElectionStatus Status { get; set; } = ElectionStatus.Pending;
        public bool IsPrivate { get; set; }
        public Guid OrganizationId { get; set; }
        public List<ElectionCategoryDto> Categories { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
