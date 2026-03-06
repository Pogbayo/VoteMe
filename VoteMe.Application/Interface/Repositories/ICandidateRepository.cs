using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interfaces.Repositories
{
    public interface ICandidateRepository : IGenericRepository<Candidate>
    {
        Task<IEnumerable<Candidate>> GetElectionCandidatesAsync(Guid electionId);
        Task<Candidate?> GetWithVotesAsync(Guid candidateId);
        Task<int> GetVoteCountAsync(Guid candidateId);
    }
}