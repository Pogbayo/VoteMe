using VoteMe.Domain.Entities;

namespace VoteMe.Application.Events.Voting
{
    public class VoteCastEvent
    {
        public Guid ElectionId { get; set; }
        public string ElectionTitle { get; set; } = string.Empty;
        public Guid CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string VoterEmail { get; set; } = string.Empty;
        public string VoterFullName { get; set; } = string.Empty;
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    }
}