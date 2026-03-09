using Microsoft.Extensions.Logging;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.DTOs.OrganizationMember;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Application.Services
{
    public class OrganizationMemberService : IOrganizationMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrganizationMemberService> _logger;
        public OrganizationMemberService(IUnitOfWork unitOfWork, ILogger<OrganizationMemberService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public Task<ApiResponse<bool>> DemoteFromAdminAsync(Guid organizationId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<OrganizationMemberDto>>> GetMembersAsync(Guid organizationId, int page = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<OrganizationDto>>> GetUserOrganizationsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> JoinOrganizationAsync(Guid userId, string uniqueKey)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> LeaveOrganizationAsync(Guid organizationId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> PromoteToAdminAsync(Guid organizationId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> RemoveMemberAsync(Guid organizationId, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
