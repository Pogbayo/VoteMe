
namespace VoteMe.Application.Events.Election
{
    public class ElectionClosedEvent
    {
        public Guid ElectionId { get; set; }
        public Guid OrganizationId { get; set; }
        public string ElectionTitle { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string WinnerName { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public List<string> MemberEmails { get; set; } = new();
    }
}
