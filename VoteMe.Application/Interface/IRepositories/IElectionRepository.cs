using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;

namespace VoteMe.Application.Interface.IRepositories
{
    public interface IElectionRepository : IGenericRepository<Election>
    {
        Task<Election?> GetFullElectionAsync(Guid electionId);
        Task<(IEnumerable<Election> Items, int TotalCount)> GetOrganizationElectionsAsync(
            Guid organizationId,
            int page = 1,
            int pageSize = 20);
        Task<IEnumerable<Election>> GetElectionsByStatusAsync(ElectionStatus status);
        Task<IEnumerable<Election>> GetActiveElectionsAsync();
    }
}