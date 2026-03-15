namespace VoteMe.Application.Events.Voting
{
    public class VoteChangedEvent
    {
        public Guid ElectionId { get; set; }
        public string ElectionName { get; set; } = string.Empty;
        public Guid ElectionCategoryId { get; set; }
        public Guid OldCandidateId { get; set; }
        public Guid NewCandidateId { get; set; }
        public string NewCandidateFirstName { get; set; } = string.Empty;
        public string NewCandidateLastName { get; set; } = string.Empty;
        public Guid? VoterId { get; set; }
        public string VoterEmail { get; set; } = string.Empty;
        public string VoterDisplayName { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
