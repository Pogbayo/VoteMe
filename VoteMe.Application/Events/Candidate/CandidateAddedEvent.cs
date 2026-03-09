namespace VoteMe.Application.Events.Candidate
{
    public class CandidateAddedEvent
    {
        public Guid CandidateId { get; set; }
        public Guid AddedByUserId { get; set; }
        public string CandidateFirstName { get; set; } = string.Empty;
        public string CandidateLastName { get; set; } = string.Empty;
        public string? CandidateDisplayName { get; set; }
        public string ElectionCategoryName { get; set; } = string.Empty;
        public string ElectionName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public List<string> MemberEmails { get; set; } = new();
    }
}
