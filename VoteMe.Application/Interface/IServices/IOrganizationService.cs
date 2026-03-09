using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Organization;

namespace VoteMe.Application.Interface.IServices
{
    public interface IOrganizationService
    {
        Task<ApiResponse<OrganizationDto>> GetOrganizationAsync(Guid organizationId);
        void UpdateOrganizationAsync(Guid organizationId, UpdateOrganizationDto dto);
        Task<ApiResponse<bool>> DeleteOrganizationAsync(Guid organizationId);
    }
}
