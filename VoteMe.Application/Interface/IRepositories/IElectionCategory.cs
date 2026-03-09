using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interface.IRepositories
{
    public interface IElectionCategoryRepository : IGenericRepository<ElectionCategory>
    {
        Task<ElectionCategory?> GetWithCandidatesAsync(Guid categoryId);
        Task<IEnumerable<ElectionCategory>> GetElectionCategoriesAsync(Guid electionId);
        Task<ElectionCategory?> GetWithVotesAsync(Guid categoryId);
    }
}
