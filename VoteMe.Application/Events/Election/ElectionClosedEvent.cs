using VoteMe.Application.DTOs.ElectionCategory;

namespace VoteMe.Application.Events.Election
{
    public class ElectionClosedEvent
    {
        public Guid ElectionId { get; set; }
        public Guid ClosedByUserId { get; set; }
        public string ElectionName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public List<string> MemberEmails { get; set; } = new();
        public List<ElectionCategoryResultDto> CategoryResults { get; set; } = new();

    }
}