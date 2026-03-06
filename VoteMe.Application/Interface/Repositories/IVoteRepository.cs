using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interfaces.Repositories
{
    public interface IVoteRepository : IGenericRepository<Vote>
    {
        Task<bool> HasUserVotedAsync(Guid userId, Guid electionId);
        Task<IEnumerable<Vote>> GetElectionVotesAsync(Guid electionId);
        Task<Dictionary<Guid, int>> GetVoteCountsAsync(Guid electionId);
        Task<int> GetTotalVotesAsync(Guid electionId);
        Task<Vote?> ChangeVoteAsync(Guid userId, Guid electionId, Guid newCandidateId);
    }
}