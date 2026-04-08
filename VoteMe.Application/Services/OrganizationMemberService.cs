using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.DTOs.OrganizationMember;
using VoteMe.Application.Events.Organization;
using VoteMe.Application.Helpers;
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
        private readonly UserManager<AppUser> _userManager;
        public OrganizationMemberService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMessageBus messageBus,
            ICacheService cacheService,
            UserManager<AppUser> userManager,
            ILogger<OrganizationMemberService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _messageBus = messageBus;
            _userManager = userManager;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<bool>> PromoteToAdminAsync(Guid organizationId, Guid userId)
        {
            var requester = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId,_currentUserService.UserId);

            if (requester == null)
                throw new BadRequestException("User not found");

            if (!PermissionChecker.HasPermission(requester.Role, Permission.PromoteToAdmin))
                throw new ForbiddenException("You do not have permission to promote members");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId, userId);
            if (member == null)
                throw new BadRequestException("Member not found in this organization");

            if (member.Role == OrganizationRole.Admin)
                throw new BadRequestException("Member is already an admin");

            member.Role = OrganizationRole.Admin;
            member.UpdateTimestamps();

            _unitOfWork.OrganizationMembers.Update(member);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"OrganizationMembers_{organizationId}");

            _logger.LogInformation("User {UserId} promoted to admin in organization {OrganizationId}", userId, organizationId);

            await _unitOfWork.AuditLogs.LogAsync(
                _currentUserService.UserId,
                AuditAction.PromoteToAdmin,
                $"User {userId} promoted to admin in organization {organizationId}"
            );

            return ApiResponse<bool>.SuccessResponse(true, "Member promoted to admin successfully");
        }
        public async Task<ApiResponse<bool>> DemoteFromAdminAsync(Guid organizationId, Guid userId)
        {
            var requester = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId, _currentUserService.UserId);

            if (requester == null)
                throw new BadRequestException("User not found");

            if (!PermissionChecker.HasPermission(requester.Role, Permission.PromoteToAdmin))
                throw new ForbiddenException("You do not have permission to demote members");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId, userId);
            if (member == null)
                throw new BadRequestException("Member not found in this organization");

            if (member.Role != OrganizationRole.Admin)
                throw new BadRequestException("User is not an admin");

            var adminCount = await _unitOfWork.OrganizationMembers
                .CountAsync(m => m.OrganizationId == organizationId && m.Role == OrganizationRole.Admin);

            if (adminCount <= 1)
                throw new BadRequestException("Organization must have at least one admin");

            member.Role = OrganizationRole.Member;
            member.UpdateTimestamps();

            _unitOfWork.OrganizationMembers.Update(member);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"OrganizationMembers_{organizationId}");

            await _unitOfWork.AuditLogs.LogAsync(
                _currentUserService.UserId,
                AuditAction.DemoteFromAdmin,
                $"User {userId} demoted from admin in organization {organizationId}"
            );

            return ApiResponse<bool>.SuccessResponse(true, "User demoted from admin successfully");
        }
        public async Task<ApiResponse<bool>> RemoveMemberAsync(Guid organizationId, Guid userId)
        {
            var requester = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId, _currentUserService.UserId);

            if (requester == null)
                throw new BadRequestException("User not found");

            if (!PermissionChecker.HasPermission(requester.Role, Permission.RemoveMember))
                throw new ForbiddenException("You do not have permission to remove members");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId, userId);

            if (member == null)
                throw new BadRequestException("Member not found in this organization");

            if (PermissionChecker.HasPermission(member.Role, Permission.ApproveMember))
                throw new BadRequestException("Cannot remove a user with elevated permissions — demote them first");

            member.Status = MembershipStatus.Removed;
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"OrganizationMembers_{organizationId}");

            _logger.LogInformation("User {UserId} removed from organization {OrganizationId} by {RequesterId}",
                userId, organizationId, requester.UserId);

            await _messageBus.PublishAsync("member-removed", new MemberRemovedFromOrganizationEvent
            {
                RemovedByUserId = requester.UserId,
                RemovedUserId = userId,
                DisplayName = member.DisplayName,
                OrganizationName = member.Organization.Name
            });

            return ApiResponse<bool>.SuccessResponse(true, "Member removed successfully");
        }
        public async Task<ApiResponse<bool>> ApproveMemberAsync(Guid organizationId, Guid userId)
        {
            var requester = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId, _currentUserService.UserId);

            if (requester == null)
                throw new BadRequestException("User not found");

            if (!PermissionChecker.HasPermission(requester.Role, Permission.ApproveMember))
                throw new ForbiddenException("You do not have permission to approve members");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId, userId);
            if (member == null)
                throw new BadRequestException("Member not found");

            if (member.Status == MembershipStatus.Approved)
                throw new BadRequestException("Member is already approved");

            member.Status = MembershipStatus.Approved;
            member.UpdateTimestamps();

            _unitOfWork.OrganizationMembers.Update(member);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"OrganizationMembers_{organizationId}");

            return ApiResponse<bool>.SuccessResponse(true, "Member approved successfully");
        }
        public async Task<ApiResponse<bool>> RejectMemberAsync(Guid organizationId, Guid userId)
        {
            var requester = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId, _currentUserService.UserId);

            if (requester == null)
                throw new BadRequestException("User not found");

            if (!PermissionChecker.HasPermission(requester.Role, Permission.ApproveMember))
                throw new ForbiddenException("You do not have permission to reject members");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId, userId);
            if (member == null)
                throw new BadRequestException("Member not found");

            if (member.Status == MembershipStatus.Rejected)
                throw new BadRequestException("Member is already rejected");

            member.Status = MembershipStatus.Rejected;
            member.UpdateTimestamps();

            _unitOfWork.OrganizationMembers.Update(member);
            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"OrganizationMembers_{organizationId}");

            return ApiResponse<bool>.SuccessResponse(true, "Member rejected successfully");
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
        public async Task<ApiResponse<OrganizationMemberDto>> JoinOrganizationAsync(JoinOrgDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UniqueKey))
                throw new BadRequestException("Unique key is required");

            var userId = _currentUserService.UserId;

            var organization = await _unitOfWork.Organizations.GetByUniqueKeyAsync(dto.UniqueKey.Trim().ToUpper());
            if (organization == null)
                throw new BadRequestException("Organization not found");

            var alreadyMember = await _unitOfWork.OrganizationMembers
                .IsMemberAsync(userId, organization.Id);

            if (alreadyMember)
                throw new BadRequestException("You are already a member of this organization");
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new BadRequestException("User not found");

            var member = new OrganizationMember
            {
                UserId = userId,
                User = user,
                Organization = organization,
                OrganizationId = organization.Id,
                DisplayName = dto.DisplayName?.Trim()!,
                Role = OrganizationRole.Member,
                Status = MembershipStatus.Pending,       
                JoinedAt = DateTime.UtcNow
            };

            var success = await _unitOfWork.OrganizationMembers.JoinOrganizationAsync(userId, dto, organization.Id);

            if (!success)
                throw new Domain.Exceptions.InvalidOperationException("Failed to join organization");

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} submitted join request to organization '{OrgName}'",
                userId, organization.Name);

            await _messageBus.PublishAsync("member-joined", new MemberJoinedOrganizationEvent
            {
                UserId = userId,
                Email = _currentUserService.Email,
                DisplayName = member.DisplayName,
                OrganizationName = organization.Name,
            });

            return ApiResponse<OrganizationMemberDto>.SuccessResponse(
                OrganizationMapper.ToMemberDto(member),
                "Join request submitted successfully. Awaiting admin approval.");
        }
        public async Task<ApiResponse<bool>> LeaveOrganizationAsync(Guid organizationId)
        {
            var userId = _currentUserService.UserId;

            var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
            if (organization == null || organization.IsDeleted)
                throw new BadRequestException("Organization not found");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId, userId);

            if (member == null || member.Status == MembershipStatus.Removed)
                throw new BadRequestException("You are not an active member of this organization");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                member.Status = MembershipStatus.Removed;
                member.DeletedAt = DateTime.UtcNow;
                _unitOfWork.OrganizationMembers.Update(member);

                await _unitOfWork.SaveChangesAsync();

                var activeMemberCount = await _unitOfWork.OrganizationMembers
                    .CountAsync(m => m.OrganizationId == organizationId
                                  && m.Status != MembershipStatus.Removed);

                if (activeMemberCount == 0)
                {
                    organization.IsActive = false;
                    organization.IsDeleted = true;
                    organization.DeletedAt = DateTime.UtcNow;
                    _unitOfWork.Organizations.Update(organization);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Organization {OrganizationId} was automatically deactivated - last member left", organizationId);
                }

                await _unitOfWork.CommitTransactionAsync();

                await _cacheService.RemoveAsync($"organization-members-{organizationId}");
                await _cacheService.RemoveAsync($"user-organizations-{userId}");

                await _messageBus.PublishAsync("member-left", new MemberLeftOrganizationEvent
                {
                    UserId = userId,
                    Email = _currentUserService.Email,
                    DisplayName = member.DisplayName!,
                    OrganizationName = organization.Name,
                    //OrganizationId = organizationId
                });

                return ApiResponse<bool>.SuccessResponse(true, "Successfully left the organization");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Failed to leave organization {OrganizationId}", organizationId);
                throw;
            }
        }
        public async Task<ApiResponse<IEnumerable<PendingMemberDto>>> GetPendingMembersAsync(Guid organizationId)
        {
            var requester = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId, _currentUserService.UserId);

            if (requester == null)
                throw new BadRequestException("User not found");

            //if (!PermissionChecker.HasPermission(requester.Role, Permission.ApproveMember))
            //    throw new ForbiddenException("You do not have permission to view pending members");

            var pendingMembers = await _unitOfWork.OrganizationMembers
                .GetMembersByStatusAsync(organizationId, MembershipStatus.Pending);

            var dtos = pendingMembers.Select(m => new PendingMemberDto
            {
                UserId = m.UserId,
                FirstName = m.User.FirstName,
                LastName = m.User.LastName,
                DisplayName = m.DisplayName ?? $"{m.User.FirstName} {m.User.LastName}",
                Email = m.User.Email ?? string.Empty,
                JoinedAt = m.JoinedAt,
                Status = m.Status
            });

            return ApiResponse<IEnumerable<PendingMemberDto>>.SuccessResponse(dtos, "Pending members retrieved successfully");
        }
        public async Task<ApiResponse<OrganizationMemberDto>> GetMemberShip(Guid organizationId, Guid userId)
        {
            if(userId  == Guid.Empty) throw new BadRequestException("userId is null");
            if(organizationId  == Guid.Empty) throw new BadRequestException("organizationId is null");

            var memberShip = await _unitOfWork.OrganizationMembers.GetMemberAsync(organizationId, userId);
            if (memberShip == null) throw new BadRequestException("No existing membership between User and Org.");
            _logger.LogInformation($"Status before mapping: {memberShip.Status}");
            var dto = OrganizationMapper.ToMemberDto(memberShip);
            _logger.LogInformation($"Status after mapping: {dto.Status.ToString()}");
            return ApiResponse<OrganizationMemberDto>.SuccessResponse(dto);           
        }
        public async Task<ApiResponse<int>> GetApprovedMembersCount(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                throw new BadRequestException("OrgId cannot be null");

            var userId = _currentUserService.UserId;

            var userCount = await _unitOfWork.OrganizationMembers.GetApprovedMembersCount(organizationId, userId);
            return ApiResponse<int>.SuccessResponse(userCount);
        }
        public async Task<ApiResponse<int>> GetPendingMembersCount(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                throw new BadRequestException("OrgId cannot be null");

            var userId = _currentUserService.UserId;

            var userCount = await _unitOfWork.OrganizationMembers.GetPendingMembersCount(organizationId, userId);
            return ApiResponse<int>.SuccessResponse(userCount);
        }
    }
}