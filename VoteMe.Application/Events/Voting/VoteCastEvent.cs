
namespace VoteMe.Application.Events.Voting
{
    public class VoteCastEvent
    {
        public Guid ElectionId { get; set; }
        public string ElectionName { get; set; } = string.Empty;
        public Guid ElectionCategoryId { get; set; }
        public string ElectionCategoryName { get; set; } = string.Empty;
        public Guid CandidateId { get; set; }
        public string CandidateFirstName { get; set; } = string.Empty;
        public string CandidateLastName { get; set; } = string.Empty;
        public string? CandidateDisplayName { get; set; }
        public Guid? VoterId { get; set; }
        public string VoterFirstName { get; set; } = string.Empty;
        public string VoterLastName { get; set; } = string.Empty;
        public string VoterDisplayName { get; set; } = string.Empty;
        public string VoterEmail { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    }
}