using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VoteMe.Application.Authorization;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.Events.Organization;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Application.Mappers.Organization;
using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;
using VoteMe.Domain.Exceptions;

namespace VoteMe.Application.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrganizationService> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMessageBus _messageBus;
        private readonly ICacheService _cacheService;
        private readonly UserManager<AppUser> _userManager;
        public OrganizationService(IUnitOfWork unitOfWork,UserManager<AppUser> userManager, ICurrentUserService currentUserService, IMessageBus messageBus, ICacheService cacheService, ILogger<OrganizationService> logger)
        {
            _logger = logger;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _messageBus = messageBus;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<bool>> DeleteOrganizationAsync(Guid organizationId)
        {
            var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
            if (organization == null)
                throw new NotFoundException("Organization not found");

            await OrganizationAuthorization.RequireCurrentUserIsOrgAdmin(
                _unitOfWork,
                _currentUserService,
                organizationId,
                "delete this organization");

            var hasActiveElections = await _unitOfWork.Elections.ExistsAsync(
                e => e.OrganizationId == organizationId &&
                     e.Status == ElectionStatus.Active);

            if (hasActiveElections)
                throw new BadRequestException("Cannot delete an organization with active elections. Close them first.");

            await _unitOfWork.Organizations.SoftDeleteByIdAsync(organizationId);

            var memberEmails = await _unitOfWork.OrganizationMembers.GetOrganizationMemberEmailsAsync(organizationId);

            await _unitOfWork.CascadeSoftDeleteForOrganizationAsync(organizationId);

            organization.MarkAsDeleted();
            _unitOfWork.Organizations.Update(organization);

            await _unitOfWork.SaveChangesAsync();

            await _messageBus.PublishAsync("organization-deleted", new OrganizationDeletedEvent
            {
                OrganizationId = organizationId,
                OrganizationName = organization.Name,
                DeletedByUserId = _currentUserService.UserId
            });

            await _cacheService.RemoveAsync($"organization-{organizationId}");

            return ApiResponse<bool>.SuccessResponse(true, "Organization soft-deleted successfully");
        }

        public async Task<ApiResponse<OrganizationDto>> GetOrganizationAsync(Guid organizationId)
        {
            var cacheKey = $"organization-{organizationId}";
            var cached = await _cacheService.GetAsync<OrganizationDto>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<OrganizationDto>.SuccessResponse(
                    cached,
                    "Organization retrieved successfully (from cache)");
            }

            var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
            if (organization == null || organization.IsDeleted)
                throw new NotFoundException("Organization not found");

            var dto = OrganizationMapper.ToDto(organization);

            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15));

            return ApiResponse<OrganizationDto>.SuccessResponse(
                dto,
                "Organization retrieved successfully");
        }

        public async Task<ApiResponse<bool>> UpdateOrganizationAsync(
            Guid organizationId,
            UpdateOrganizationDto dto)
        {
            var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
            if (organization == null || organization.IsDeleted)
                throw new NotFoundException("Organization not found");
            var userId = _currentUserService.UserId;
            await OrganizationAuthorization.RequireCurrentUserIsOrgAdmin(
                _unitOfWork,
                _currentUserService,
                organizationId,
                "update this organization");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                organization.Name = dto.Name.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Description))
                organization.Description = dto.Description.Trim();

            if (dto.Logo == null)
                organization.LogoUrl = organization.LogoUrl;

           
            organization.UpdateTimestamps();

            _unitOfWork.Organizations.Update(organization);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"organization-{organizationId}");
            await _unitOfWork.AuditLogs.LogAsync(userId, AuditAction.Update, $"User {userId} updated organization {organizationId}");

            return ApiResponse<bool>.SuccessResponse(true);
        }
    }
}
