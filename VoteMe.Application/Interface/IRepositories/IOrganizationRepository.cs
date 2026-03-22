using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interface.IRepositories
{
    public interface IOrganizationRepository : IGenericRepository<Organization>
    {
        Task<Organization?> GetByUniqueKeyAsync(string uniqueKey);
        //Task<Organization?> GetByEmailAsync(string email);
        //Task<Organization?> GetOrgByIdAsync(Guid organizationId);
        Task<Organization?> GetWithMembersAsync(Guid organizationId);
        Task<Organization?> GetWithElectionsAsync(Guid organizationId);
        //Task<Organization?> GetFullOrganization(Guid organizationId);
        Task<bool> UniqueKeyExistsAsync(string uniqueKey);
    }
}