using VoteMe.Domain.Entities;

namespace VoteMe.Application.Interfaces.Repositories
{
    public interface IOrganizationMemberRepository : IGenericRepository<OrganizationMember>
    {
        Task<OrganizationMember?> GetMemberAsync(Guid userId, Guid organizationId);
        Task<IEnumerable<OrganizationMember>> GetOrganizationMembersAsync(Guid organizationId);
        Task<IEnumerable<OrganizationMember>> GetUserOrganizationsAsync(Guid userId);
        Task<bool> IsMemberAsync(Guid userId, Guid organizationId);
        Task<bool> IsAdminAsync(Guid userId, Guid organizationId);
    }
}