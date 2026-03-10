using VoteMe.Domain.Entities;
namespace VoteMe.Application.Interface.IRepositories
{
    public interface IElectionCategoryRepository : IGenericRepository<ElectionCategory>
    {   
        Task<ElectionCategory?> GetElectionCategoryAsync(Guid categoryId);
        Task<IEnumerable<ElectionCategory>> GetElectionCategoriesAsync(Guid electionId);
        Task<ElectionCategory?> GetElectionCategoryResultsAsync(Guid categoryId);
    }
}

