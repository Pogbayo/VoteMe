using Microsoft.Extensions.Logging;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.DTOs.OrganizationMember;
using VoteMe.Application.Events.Organization;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Application.Mappers.Organization;
using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;
using VoteMe.Domain.Exceptions;

namespace VoteMe.Application.Services
{
    public class OrganizationMemberService : IOrganizationMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrganizationMemberService> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMessageBus _messageBus;
        private readonly ICacheService _cacheService;

        public OrganizationMemberService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMessageBus messageBus, ICacheService cacheService, ILogger<OrganizationMemberService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _messageBus = messageBus;
            _cacheService = cacheService;
        }
        public async Task<ApiResponse<bool>> DemoteFromAdminAsync(Guid organizationId, Guid userId)
        {
            var organization = await _unitOfWork.Organizations
                .FindOneAsync(o => o.Id == organizationId);

            if (organization == null)
                throw new NotFoundException("Organization not found");

            var member = await _unitOfWork.OrganizationMembers
                .FindOneAsync(m => m.OrganizationId == organizationId && m.UserId == userId);

            if (member == null)
                throw new NotFoundException("Member not found in the organization");

            if (!member.IsAdmin)
                throw new BadRequestException("User is not an admin");

            var adminCount = await _unitOfWork.OrganizationMembers
                .CountAsync(m => m.OrganizationId == organizationId && m.IsAdmin);

            if (adminCount <= 1)
                throw new BadRequestException("Organization must have at least one admin");

            member.IsAdmin = false;
            member.UpdateTimestamps();

            _unitOfWork.OrganizationMembers.Update(member);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.AuditLogs.AddAsync(new AuditLog
            {
                UserId = _currentUserService.UserId,
                Action = AuditAction.DemoteFromAdmin,
                Details = $"User {userId} demoted from admin in organization {organizationId}",
                Timestamp = DateTime.UtcNow
            });

            return ApiResponse<bool>.SuccessResponse(true, "User demoted from admin successfully");
        }

        public async Task<ApiResponse<IEnumerable<OrganizationMemberDto>>> GetMembersAsync(Guid organizationId, int page = 1, int pageSize = 20)
        {
            var cacheKey = $"OrganizationMembers_{organizationId}_Page{page}_Size{pageSize}";
            var cachedData = await _cacheService.GetAsync<IEnumerable<OrganizationMemberDto>>(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("Returning cached members for organization {OrganizationId}", organizationId);
                return ApiResponse<IEnumerable<OrganizationMemberDto>>.SuccessResponse(cachedData, "Members retrieved from cache");
            }

            var members = await _unitOfWork.OrganizationMembers.GetOrganizationMembersAsync(organizationId, page, pageSize);
            var memberDtos = OrganizationMapper.ToMemberDtoList(members);

            await _cacheService.SetAsync(cacheKey, memberDtos, TimeSpan.FromMinutes(5));

            return ApiResponse<IEnumerable<OrganizationMemberDto>>.SuccessResponse(memberDtos, "Members retrieved successfully");
        }

        public async Task<ApiResponse<IEnumerable<OrganizationDto>>> GetUserOrganizationsAsync()
        {
            var userId = _currentUserService.UserId;

            var memberships = await _unitOfWork.OrganizationMembers
                .GetUserMembershipsAsync(userId);

            var organizations = memberships.Select(m => m.Organization);

            return ApiResponse<IEnumerable<OrganizationDto>>.SuccessResponse(
                OrganizationMapper.ToDtoList(organizations),
                "Organizations retrieved successfully");
        }

        public async Task<ApiResponse<bool>> JoinOrganizationAsync(string uniqueKey)
        {
            if (string.IsNullOrWhiteSpace(uniqueKey))
                throw new BadRequestException("Unique key is required");

            var userId = _currentUserService.UserId;

            var organization = await _unitOfWork.Organizations.GetByUniqueKeyAsync(uniqueKey.Trim().ToUpper());
            if (organization == null)
                throw new NotFoundException("Organization not found");

            var alreadyMember = await _unitOfWork.OrganizationMembers
                .IsMemberAsync(userId, organization.Id);

            if (alreadyMember)
                throw new BadRequestException("You are already a member of this organization");

            var member = new OrganizationMember
            {
                UserId = userId,
                OrganizationId = organization.Id,
                IsAdmin = false,
                JoinedAt = DateTime.UtcNow,
                Status = MembershipStatus.Pending
            };

            await _unitOfWork.OrganizationMembers.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} joined organization '{OrgName}'",
                userId, organization.Name);

            await _messageBus.PublishAsync("member-joined", new MemberJoinedOrganizationEvent
            {
                UserId = userId,
                Email = _currentUserService.Email,
                DisplayName = _currentUserService.DisplayName,
                OrganizationName = organization.Name
            });

            return ApiResponse<bool>.SuccessResponse(true, "Join request submitted. Awaiting admin approval");
        }

        public async Task<ApiResponse<bool>> LeaveOrganizationAsync(Guid organizationId)
        {
            var userId = _currentUserService.UserId;

            var organization = await _unitOfWork.Organizations
                .FindOneAsync(o => o.Id == organizationId);
            if (organization == null)
                throw new NotFoundException("Organization not found");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(userId, organizationId);
            if (member == null)
                throw new NotFoundException("You are not a member of this organization");

            var remainingMemberCount = await _unitOfWork.OrganizationMembers
                .CountAsync(m => m.OrganizationId == organizationId);

            if (member.IsAdmin && remainingMemberCount > 1)
                throw new BadRequestException("Admins cannot leave the organization — transfer admin rights first");

            await _unitOfWork.OrganizationMembers.SoftDeleteByIdAsync(member.Id);

            if (remainingMemberCount <= 1)
            {
                organization.IsActive = false;
                organization.IsDeleted = true;
                organization.DeletedAt = DateTime.UtcNow;
                _unitOfWork.Organizations.Update(organization);

                _logger.LogInformation(
                    "Organization {OrganizationId} deleted — last member {UserId} left",
                    organizationId, userId);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} left organization {OrganizationId}",
                userId, organizationId);

            await _messageBus.PublishAsync("member-left", new MemberLeftOrganizationEvent
            {
                UserId = userId,
                Email = _currentUserService.Email,
                DisplayName = _currentUserService.DisplayName,
                OrganizationName = organization.Name
            });

            return ApiResponse<bool>.SuccessResponse(true, "Successfully left organization");
        }
        public async Task<ApiResponse<bool>> PromoteToAdminAsync(Guid organizationId, Guid userId)
        {
            var requesterId = _currentUserService.UserId;

            var requester = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(requesterId, organizationId);
            if (requester == null || !requester.IsAdmin)
                throw new ForbiddenException("Only admins can promote members");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(userId, organizationId);
            if (member == null)
                throw new NotFoundException("Member not found in this organization");

            if (member.IsAdmin)
                throw new BadRequestException("Member is already an admin");

            member.IsAdmin = true;
            member.UpdateTimestamps();

            _unitOfWork.OrganizationMembers.Update(member);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} promoted to admin in organization {OrganizationId}",
                userId, organizationId);

            await _unitOfWork.AuditLogs.LogAsync(requesterId,AuditAction.PromoteToAdmin, $"User {userId} promoted to admin in organization {organizationId}");

            return ApiResponse<bool>.SuccessResponse(true, "Member promoted to admin successfully");
        }

        public async Task<ApiResponse<bool>> RemoveMemberAsync(Guid organizationId, Guid userId)
        {
            var requesterId = _currentUserService.UserId;

            var requester = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(requesterId, organizationId);
            if (requester == null)
                throw new ForbiddenException("You need to be a memeber of this organization to remove a member");

            if (!requester.IsAdmin)
                throw new ForbiddenException("Only admins can remove members");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(userId, organizationId);
            if (member == null)
                throw new NotFoundException("Member not found in this organization");

            if (member.IsAdmin)
                throw new BadRequestException("Cannot remove an admin — demote them first");

            //await _unitOfWork.OrganizationMembers.SoftDeleteByIdAsync(member.Id);
            member.Status = MembershipStatus.Removed;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} removed from organization {OrganizationId} by {RequesterId}",
                userId, organizationId, requesterId);

            await _messageBus.PublishAsync("member-removed", new MemberRemovedFromOrganizationEvent
            {
                RemovedByUserId = requesterId,
                RemovedUserId = userId,
                DisplayName = member.User.DisplayName ?? $"{member.User.FirstName} {member.User.LastName}",
                OrganizationName = string.Empty
            });

            return ApiResponse<bool>.SuccessResponse(true, "Member removed successfully");
        }

        public async Task<ApiResponse<bool>> ApproveMemberAsync(Guid organizationId, Guid userId)
        {
            var requesterId = _currentUserService.UserId;
            var requester = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(requesterId, organizationId);

            if (requester == null || !requester.IsAdmin)
                throw new ForbiddenException("Only admins can approve members");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(userId, organizationId);
            if (member == null)
                throw new NotFoundException("Member not found");

            if (member.Status == MembershipStatus.Approved)
                throw new BadRequestException("Member is already approved");

            member.Status = MembershipStatus.Approved;
            member.UpdateTimestamps();

            _unitOfWork.OrganizationMembers.Update(member);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"OrganizationMembers_{organizationId}");

            _logger.LogInformation("User {UserId} approved in organization {OrganizationId}",
                userId, organizationId);

            return ApiResponse<bool>.SuccessResponse(true, "Member approved successfully");
        }

        public async Task<ApiResponse<IEnumerable<PendingMemberDto>>> GetPendingMembersAsync(Guid organizationId)
        {
            var requesterId = _currentUserService.UserId;

            _logger.LogInformation("RequesterId from token: {RequesterId}", requesterId);
            _logger.LogInformation("OrganizationId: {OrganizationId}", organizationId);

            var requester = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(requesterId, organizationId);

            _logger.LogInformation("Requester found: {Found}, IsAdmin: {IsAdmin}",
                requester != null, requester?.IsAdmin);

            if (requester == null || !requester.IsAdmin)
                throw new ForbiddenException("Only admins can view pending members");

            var pendingMembers = await _unitOfWork.OrganizationMembers
                .GetMembersByStatusAsync(organizationId, MembershipStatus.Pending);

            var dtos = pendingMembers.Select(m => new PendingMemberDto
            {
                UserId = m.UserId,
                FirstName = m.User.FirstName,
                LastName = m.User.LastName,
                DisplayName = m.User.DisplayName ?? $"{m.User.FirstName} {m.User.LastName}",
                Email = m.User.Email ?? string.Empty,
                JoinedAt = m.JoinedAt,
                Status = m.Status
            });

            return ApiResponse<IEnumerable<PendingMemberDto>>.SuccessResponse(
                dtos, "Pending members retrieved successfully");
        }

        public async Task<ApiResponse<bool>> RejectMemberAsync(Guid organizationId, Guid userId)
        {
            var requesterId = _currentUserService.UserId;
            var requester = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(requesterId, organizationId);

            if (requester == null || !requester.IsAdmin)
                throw new ForbiddenException("Only admins can reject members");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(userId, organizationId);
            if (member == null)
                throw new NotFoundException("Member not found");

            if (member.Status == MembershipStatus.Rejected)
                throw new BadRequestException("Member is already rejected");

            member.Status = MembershipStatus.Rejected;
            member.UpdateTimestamps();

            _unitOfWork.OrganizationMembers.Update(member);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"OrganizationMembers_{organizationId}");

            return ApiResponse<bool>.SuccessResponse(true, "Member rejected successfully");
        }
    }
}
