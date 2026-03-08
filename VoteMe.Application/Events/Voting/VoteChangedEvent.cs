namespace VoteMe.Application.Events.Voting
{
    public class VoteChangedEvent
    {
        public Guid ElectionId { get; set; }
        public Guid NewCandidateId { get; set; }
        public Guid UserId { get; set; }
        public string VoterEmail { get; set; } = string.Empty;
        public string VoterFullName { get; set; } = string.Empty;
        public string ElectionTitle { get; set; } = string.Empty;
        public string NewCandidateName { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
