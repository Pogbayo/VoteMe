using VoteMe.Application.Common.VoteMe.Application.Common;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.DTOs.OrganizationMember;

namespace VoteMe.Application.Interface.IServices
{
    public interface IOrganizationMemberService
    {
        Task<ApiResponse<bool>> RemoveMemberAsync(Guid organizationId, Guid userId);
        Task<ApiResponse<bool>> JoinOrganizationAsync(Guid userId, string uniqueKey);
        Task<ApiResponse<bool>> PromoteToAdminAsync(Guid organizationId, Guid userId);
        Task<ApiResponse<bool>> DemoteFromAdminAsync(Guid organizationId, Guid userId);
        Task<ApiResponse<bool>> LeaveOrganizationAsync(Guid organizationId, Guid userId);
        Task<ApiResponse<IEnumerable<OrganizationDto>>> GetUserOrganizationsAsync(Guid userId);
        Task<ApiResponse<IEnumerable<OrganizationMemberDto>>> GetMembersAsync(Guid organizationId, int page = 1, int pageSize = 20);
    }
}
