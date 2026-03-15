using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interface.IRepositories
{
    public interface IVoteRepository : IGenericRepository<Vote>
    {
        Task<bool> HasUserVotedAsync(Guid userId, Guid electionCategoryId, Guid electionId);
        //Task<IEnumerable<Vote>> GetElectionVotesAsync(Guid electionId);
        Task<Dictionary<Guid, int>> GetVoteCountsAsync(Guid electionId);
        //Task<int> GetTotalVotesAsync(Guid electionId);
        Task<Vote?> ChangeVoteAsync(Guid userId, Guid electionId, Guid newCandidateId);
        Task<int> GetCandidateTotalVotesInAnElectionCategoryAsync(Guid candidateId, Guid electionCategoryId);
    }
}