using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interface.IRepositories
{
    public interface ICandidateRepository : IGenericRepository<Candidate>
    {
        Task<IEnumerable<Candidate>> GetElectionCandidatesAsync(Guid electionId);
        Task<Candidate?> GetWithVotesAsync(Guid candidateId, Guid electionId);
        Task<int> GetVoteCountAsync(Guid candidateId, Guid electionId);
    }
}