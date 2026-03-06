using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;

namespace VoteMe.Application.Interfaces.Repositories
{
    public interface IElectionRepository : IGenericRepository<Election>
    {
        Task<Election?> GetWithCandidatesAsync(Guid electionId);
        Task<Election?> GetWithVotesAsync(Guid electionId);
        Task<Election?> GetFullElectionAsync(Guid electionId);
        Task<IEnumerable<Election>> GetOrganizationElectionsAsync(Guid organizationId);
        Task<IEnumerable<Election>> GetElectionsByStatusAsync(ElectionStatus status);
        Task<IEnumerable<Election>> GetActiveElectionsAsync();
    }
}