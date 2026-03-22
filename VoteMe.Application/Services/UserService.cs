using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.User;
using VoteMe.Application.Events.User;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Application.Mappers.User;
using VoteMe.Domain.Constants;
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
        public UserService(IUnitOfWork unitOfWork,UserManager<AppUser> userManager, ICurrentUserService currentUserService, IMessageBus messageBus, ICacheService cacheService, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _messageBus = messageBus;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("Please, provide a userId");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new NotFoundException("User not found");

            if (!_currentUserService.Roles.Contains(Roles.SuperAdmin) &&
                    _currentUserService.UserId != userId)
            {
                throw new ForbiddenException("You are not authorized to delete this user");
            }

            user.MarkAsDeleted();
            user.TokenVersion++;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new Domain.Exceptions.InvalidOperationException($"Failed to soft-delete user: {errors}");
            }

            var memberships = await _unitOfWork.OrganizationMembers
                    .FindAsync(m => m.UserId == userId);

            foreach (var m in memberships)
            {
                m.MarkAsDeleted();
                _unitOfWork.OrganizationMembers.Update(m);
            }

            await _unitOfWork.SaveChangesAsync();
            await _cacheService.RemoveAsync($"user-{userId}");

            await _messageBus.PublishAsync("user-deleted", new UserDeletedEvent
            {
                UserId = userId,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                DeletedByUserId = _currentUserService.UserId
            });

            return ApiResponse<bool>.SuccessResponse(true, "User soft-deleted successfully");
        }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllUsersAsync(
              Guid organizationId,
              int page = 1,
              int pageSize = 20)
        {
            var cacheKey = $"org-users-{organizationId}-page{page}-size{pageSize}";

            var cached = await _cacheService.GetAsync<PagedUserResult>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<IEnumerable<UserDto>>.SuccessResponse(
                    cached.Users,
                    $"Retrieved {cached.Users.Count} users from cache (page {page} of {pageSize}, total {cached.TotalCount})");
            }

            var organization = await _unitOfWork.Organizations.GetWithMembersAsync(organizationId);
            if (organization == null)
                throw new NotFoundException("Organization with provided ID does not exist or has been deleted");

            var pagedMembers = await _unitOfWork.OrganizationMembers.GetPagedAsync(
                predicate: m => m.OrganizationId == organizationId,
                page,
                pageSize);

            if (!pagedMembers.Items.Any())
            {
                return ApiResponse<IEnumerable<UserDto>>.SuccessResponse(
                    Enumerable.Empty<UserDto>(),
                    "No users found in this organization");
            }

            var userDtoList = new List<UserDto>();

            foreach (var om in pagedMembers.Items)
            {
                var roles = await _userManager.GetRolesAsync(om.User);

                userDtoList.Add(new UserDto
                {
                    Id = om.User.Id,
                    FirstName = om.User.FirstName,
                    LastName = om.User.LastName,
                    DisplayName = om.User.DisplayName,
                    Email = om.User.Email ?? string.Empty,
                    Roles = roles.ToList()
                });
            }

            var totalCount = await _unitOfWork.OrganizationMembers
                .CountAsync(m => m.OrganizationId == organizationId && !m.IsDeleted);

            var cacheResult = new 
            {
                Users = userDtoList,
                TotalCount = totalCount
            };

            await _cacheService.SetAsync(cacheKey, cacheResult, TimeSpan.FromMinutes(10));
         
            return ApiResponse<IEnumerable<UserDto>>.SuccessResponse(
                userDtoList,
                $"Retrieved {userDtoList.Count} users (page {page} of {pageSize}, total {totalCount})");
        }

        public async Task<ApiResponse<UserDto>> GetUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID cannot be empty");

            var cacheKey = $"user-{userId}";

            UserDto? cached = null;
            try
            {
                cached = await _cacheService.GetAsync<UserDto>(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache read failed for key {Key}, falling through to DB", cacheKey);
            }

            if (cached != null)
                return ApiResponse<UserDto>.SuccessResponse(cached, "User retrieved successfully (from cache)");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new NotFoundException("User with provided ID not found");

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = UserMapper.ToDto(user, roles);

            try
            {
                await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(15));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache write failed for key {Key}", cacheKey);
            }

            await _unitOfWork.AuditLogs.LogAsync(
                userId: _currentUserService.UserId,
                action: AuditAction.Read,
                details: $"User {_currentUserService.UserId} retrieved user {user.Email} (ID: {user.Id})"
            );

            return ApiResponse<UserDto>.SuccessResponse(userDto, "User retrieved successfully");
        }
        public async Task<ApiResponse<UserDto>> UpdateUserAsync(Guid userId, UpdateUserDto dto)
        {
            if (userId == Guid.Empty)
                throw new BadRequestException("User ID cannot be empty");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDeleted)
                throw new NotFoundException("User not found or has been deleted");

            if (_currentUserService.UserId != userId &&
                !_currentUserService.Roles.Contains(Roles.SuperAdmin))
            {
                throw new ForbiddenException("You are not authorized to update this user");
            }

            var changes = new List<string>();

            if (!string.IsNullOrWhiteSpace(dto.FirstName) && dto.FirstName.Trim() != user.FirstName)
            {
                user.FirstName = dto.FirstName.Trim();
                changes.Add("FirstName");
            }

            if (!string.IsNullOrWhiteSpace(dto.LastName) && dto.LastName.Trim() != user.LastName)
            {
                user.LastName = dto.LastName.Trim();
                changes.Add("LastName");
            }

            if (!string.IsNullOrWhiteSpace(dto.DisplayName) && dto.DisplayName.Trim() != user.DisplayName)
            {
                user.DisplayName = dto.DisplayName.Trim();
                changes.Add("DisplayName");
            }

            user.UpdateTimestamps();

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new Domain.Exceptions.InvalidOperationException($"User update failed: {errors}");
            }

            await _unitOfWork.AuditLogs.LogAsync(
                userId: _currentUserService.UserId,
                action: AuditAction.Update,
                details: $"User {user.Email} (ID: {userId}) updated by {_currentUserService.UserId}. Changed fields: {string.Join(", ", changes)}"
            );

            await _unitOfWork.SaveChangesAsync();

            await _cacheService.RemoveAsync($"user-{userId}");

            var roles = await _userManager.GetRolesAsync(user);
            var updatedDto = UserMapper.ToDto(user, roles);

            return ApiResponse<UserDto>.SuccessResponse(
                updatedDto,
                "User updated successfully");
        }
    }
}
