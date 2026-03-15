using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interface.IRepositories
{
    public interface ICandidateRepository : IGenericRepository<Candidate>
    {
        Task<IEnumerable<Candidate>> GetCategoryCanidatesAsync(Guid electionCategoryId);
        //Task<IEnumerable<Candidate>> GetElectionCandidatesAsync(Guid electionId);
        Task<Candidate?> GetWithVotesAsync(Guid candidateId);
        Task<int> GetVoteCountAsync(Guid candidateId, Guid electionId);
    }
}