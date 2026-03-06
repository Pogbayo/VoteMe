using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interfaces.Repositories
{
    public interface IOrganizationRepository : IGenericRepository<Organization>
    {
        Task<Organization?> GetByUniqueKeyAsync(string uniqueKey);
        Task<Organization?> GetByEmailAsync(string email);
        Task<Organization?> GetWithMembersAsync(Guid organizationId);
        Task<Organization?> GetWithElectionsAsync(Guid organizationId);
        Task<bool> UniqueKeyExistsAsync(string uniqueKey);
    }
}