using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;

namespace VoteMe.Application.Interface.IRepositories
{
    public interface IOrganizationMemberRepository : IGenericRepository<OrganizationMember>
    {
        Task<OrganizationMember?> GetMemberAsync(Guid userId, Guid organizationId);
        Task<IEnumerable<OrganizationMember>> GetUserMembershipsWithOrgsAsync(Guid userId);
        Task<IEnumerable<string>> GetOrganizationMemberEmailsAsync(Guid organizationId);
        Task<IEnumerable<OrganizationMember>> GetOrganizationMembersAsync(Guid organizationId , int page , int pageSize);
        Task<IEnumerable<OrganizationMember>> GetUserOrganizationsAsync(Guid userId);
        Task<IEnumerable<OrganizationMember>> GetUserMembershipsAsync(Guid userId);
        Task<bool> IsMemberAsync(Guid userId, Guid organizationId);
        Task<bool> IsAdminAsync(Guid userId, Guid organizationId);
        Task<IEnumerable<OrganizationMember>> GetMembersByStatusAsync(Guid organizationId, MembershipStatus status);
        Task JoinOrganizationAsync(Guid userId, Guid organizationId);
    }
}