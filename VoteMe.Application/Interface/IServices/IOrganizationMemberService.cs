using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.DTOs.OrganizationMember;
using VoteMe.Domain.Enum;

namespace VoteMe.Application.Interface.IServices
{
    public interface IOrganizationMemberService
    {
        Task<ApiResponse<bool>> RemoveMemberAsync(Guid organizationId, Guid userId);
        Task<ApiResponse<OrganizationMemberDto>> JoinOrganizationAsync(JoinOrgDto dto);
        Task<ApiResponse<bool>> PromoteToAdminAsync(Guid organizationId, Guid userId);
        Task<ApiResponse<bool>> DemoteFromAdminAsync(Guid organizationId, Guid userId);
        Task<ApiResponse<bool>> LeaveOrganizationAsync(Guid organizationId);
        Task<ApiResponse<OrganizationMemberDto>> GetMemberShip(Guid organizationId,Guid userId);
        Task<ApiResponse<IEnumerable<OrganizationDto>>> GetUserOrganizationsAsync();
        Task<ApiResponse<int>> GetApprovedMembersCount(Guid organizationId);
        Task<ApiResponse<int>> GetPendingMembersCount(Guid organizationId);
        Task<ApiResponse<bool>> ApproveMemberAsync(Guid organizationId, Guid userId);
        Task<ApiResponse<bool>> RejectMemberAsync(Guid organizationId, Guid userId);
        Task<ApiResponse<IEnumerable<PendingMemberDto>>> GetPendingMembersAsync(Guid organizationId);
        Task<ApiResponse<IEnumerable<OrganizationMemberDto>>> GetMembersAsync(Guid organizationId, int page = 1, int pageSize = 20);
    }
}
