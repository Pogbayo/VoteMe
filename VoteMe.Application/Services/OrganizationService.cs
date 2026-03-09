using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Application.Services
{
    public class OrganizationService : IOrganizationService
    {
        public Task<ApiResponse<CreatedOrganizationDto>> CreateOrganizationAsync(CreateOrganizationDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteOrganizationAsync(Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<OrganizationDto>> GetOrganizationAsync(Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrganizationAsync(Guid organizationId, UpdateOrganizationDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
