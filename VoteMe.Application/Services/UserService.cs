using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.User;
using VoteMe.Application.Events.User;
using VoteMe.Application.Helpers;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Application.Mappers.User;
using VoteMe.Domain.Entities;
using VoteMe.Domain.Enum;
using VoteMe.Domain.Exceptions;

namespace VoteMe.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMessageBus _messageBus;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<AppUser> _userManager;

        public UserService(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            ICurrentUserService currentUserService,
            IMessageBus messageBus,
            ICacheService cacheService,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _currentUserService = currentUserService;
            _messageBus = messageBus;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(Guid userId, Guid organizationId)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("Please provide a valid user ID.");

            var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
            if (organization == null || organization.IsDeleted)
                throw new BadRequestException("Organization not found");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDeleted)
                throw new BadRequestException("User not found");

            var member = await _unitOfWork.OrganizationMembers
                .GetMemberAsync(organizationId,userId);

            if (member == null || member.IsDeleted)
                throw new BadRequestException("User is not a member of this organization");

            var requesterRole = await _unitOfWork.OrganizationMembers
                .GetUserRoleAsync(_currentUserService.UserId, organizationId);

            if (requesterRole == null || !PermissionChecker.HasPermission(requesterRole.Value, Permission.RemoveMember))
                throw new ForbiddenException("You do not have permission to remove this user from the organization");

            if (member.Role == OrganizationRole.Owner)
            {
                var ownerCount = await _unitOfWork.OrganizationMembers
                    .CountAsync(m => m.OrganizationId == organizationId
                                  && m.Role == OrganizationRole.Owner
                                  && !m.IsDeleted);

                if (ownerCount <= 1)
                    throw new BadRequestException("Cannot remove the last Owner. Transfer ownership first.");
            }

            member.MarkAsDeleted();
            _unitOfWork.OrganizationMembers.Update(member);

            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"organization-members-{organizationId}");
            await _cacheService.RemoveAsync($"user-organizations-{userId}");

            _logger.LogInformation("User {UserId} removed from organization '{OrgName}' by {DeletedBy}",
                userId, organization.Name, _currentUserService.UserId);

            await _messageBus.PublishAsync("user-deleted", new UserDeletedEvent
            {
                UserId = userId,
                Email = user.Email!,
                DisplayName = member.DisplayName!,
                DeletedByUserId = _currentUserService.UserId
            });

            return ApiResponse<bool>.SuccessResponse(true, "User removed from organization successfully");
        }
        public async Task<ApiResponse<IEnumerable<OrganizationUserDto>>> GetAllUsersAsync(Guid organizationId, int page = 1, int pageSize = 20)
        {
            var cacheKey = $"org-users-{organizationId}-page{page}-size{pageSize}";

            var cached = await _cacheService.GetAsync<PagedOrgUserDto>(cacheKey);
            if (cached != null)
                return ApiResponse<IEnumerable<OrganizationUserDto>>.SuccessResponse(cached.Users, $"Retrieved {cached.Users.Count} users from cache.");

            var organization = await _unitOfWork.Organizations.GetWithMembersAsync(organizationId);
            if (organization == null)
                throw new BadRequestException("Organization not found or deleted.");

            var pagedMembers = await _unitOfWork.OrganizationMembers.GetPagedAsync(
                predicate: m => m.OrganizationId == organizationId,
                page,
                pageSize);

            var users = new List<OrganizationUserDto>();
            foreach (var m in pagedMembers.Items)
            {
                users.Add(new OrganizationUserDto
                {
                    Id = m.User.Id,
                    FirstName = m.User.FirstName,
                    LastName = m.User.LastName,
                    DisplayName =  m.DisplayName,
                    Email = m.User.Email ?? string.Empty,
                    Role = m.Role 
                });
            }

            var totalCount = await _unitOfWork.OrganizationMembers.CountAsync(m => m.OrganizationId == organizationId);

            await _cacheService.SetAsync(cacheKey, new { Users = users, TotalCount = totalCount }, TimeSpan.FromMinutes(10));

            return ApiResponse<IEnumerable<OrganizationUserDto>>.SuccessResponse(users, $"Retrieved {users.Count} users (page {page})");
        }

        public async Task<ApiResponse<UserDto>> GetUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID cannot be empty.");

            var cacheKey = $"user-{userId}";
            var cached = await _cacheService.GetAsync<UserDto>(cacheKey);

            if (cached != null)
                return ApiResponse<UserDto>.SuccessResponse(cached, "User retrieved from cache.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDeleted)
                throw new BadRequestException("User not found.");

            await _cacheService.SetAsync(cacheKey, user, TimeSpan.FromMinutes(15));

            var userDto = UserMapper.ToUserDto(user);

            await _unitOfWork.AuditLogs.LogAsync(
                userId: _currentUserService.UserId,
                action: AuditAction.Read,
                details: $"User {_currentUserService.UserId} retrieved user {user.Email} (ID: {user.Id})"
            );

            return ApiResponse<UserDto>.SuccessResponse(userDto, "User retrieved successfully");
        }

        //public async Task<ApiResponse<b>> UpdateUserAsync(UpdateUserDto dto)
        //{
        //    var userId = _currentUserService.UserId;

        //    var user = await _userManager.FindByIdAsync(userId.ToString());
        //    if (user == null || user.IsDeleted)
        //        throw new BadRequestException("User not found or has been deleted.");

        //    var changes = new List<string>();

        //    await _unitOfWork.BeginTransactionAsync();

        //    if (!string.IsNullOrWhiteSpace(dto.FirstName) && dto.FirstName.Trim() != user.FirstName)
        //    {
        //        user.FirstName = dto.FirstName.Trim();
        //        changes.Add("FirstName");
        //    }

        //    if (!string.IsNullOrWhiteSpace(dto.LastName) && dto.LastName.Trim() != user.LastName)
        //    {
        //        user.LastName = dto.LastName.Trim();
        //        changes.Add("LastName");
        //    }

        //    user.UpdateTimestamps();

        //    var updateResult = await _userManager.UpdateAsync(user);
        //    await _unitOfWork.CommitTransactionAsync();
        //    await _unitOfWork.SaveChangesAsync();

        //    if (!updateResult.Succeeded)
        //    {
        //        var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
        //        throw new Domain.Exceptions.InvalidOperationException($"User update failed: {errors}");
        //    }

        //    await _unitOfWork.AuditLogs.LogAsync(
        //        userId: _currentUserService.UserId,
        //        action: AuditAction.Update,
        //        details: $"User {user.Email} (ID: {userId}) updated their profile. Changed fields: {string.Join(", ", changes)}"
        //    );

        //    await _cacheService.RemoveAsync($"user-{userId}");

        //    var updatedDto = UserMapper.ToOrgUserDto(user, member!);

        //    return ApiResponse<OrganizationUserDto>.SuccessResponse(updatedDto, "User updated successfully");
        //}
    }
}