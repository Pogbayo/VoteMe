namespace VoteMe.Application.Events.Candidate
{
    namespace VoteMe.Application.Events.Candidate
    {
        public class CandidateUpdatedEvent
        {
            public Guid CandidateId { get; set; }
            public Guid UpdatedByUserId { get; set; }
            public string CandidateFirstName { get; set; } = string.Empty;
            public string CandidateLastName { get; set; } = string.Empty;
            public string ElectionCategoryName { get; set; } = string.Empty;
            public string ElectionName { get; set; } = string.Empty;
        }
    }
}
