using Microsoft.AspNetCore.Identity;
using VoteMe.Application.Common.VoteMe.Application.Common;
using VoteMe.Application.DTOs.Auth;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.Events.Auth;
using VoteMe.Application.Events.Organization;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Domain.Constants;
using VoteMe.Domain.Entities;
using VoteMe.Domain.Exceptions;

namespace VoteMe.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMessageBus _messageBus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            INotificationService notificationService,
            IMessageBus messageBus,
            IUnitOfWork unitOfWork,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _messageBus = messageBus;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<string>> RegisterUserAsync(RegisterUserDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                    throw new BadRequestException("Email already exists");

                var organization = await _unitOfWork.Organizations.GetByUniqueKeyAsync(dto.UniqueKey);
                if (organization == null)
                    throw new BadRequestException("Invalid unique key");

                var user = new AppUser
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    UserName = dto.Email
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    throw new BadRequestException(result.Errors.First().Description);

                await _userManager.AddToRoleAsync(user, "Voter");

                var member = new OrganizationMember
                {
                    UserId = user.Id,
                    OrganizationId = organization.Id,
                    IsAdmin = false,
                    JoinedAt = DateTime.UtcNow
                };

                await _unitOfWork.OrganizationMembers.AddAsync(member);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _messageBus.PublishAsync("user-registered", new UserRegisteredEvent
                {
                    UserId = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName ?? $"{user.FirstName} {user.LastName}"

                });

                var roles = await _userManager.GetRolesAsync(user);
                var token = await _tokenService.GenerateAccessToken(user);

                return ApiResponse<string>.SuccessResponse("Registration successful");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ApiResponse<CreatedOrganizationDto>> RegisterOrganizationAsync(CreateOrganizationDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(dto.AdminEmail);
                if (existingUser != null)
                    throw new BadRequestException("Email already exists");

                var adminUser = new AppUser
                {
                    FirstName = dto.AdminFirstName,
                    LastName = dto.AdminLastName,
                    Email = dto.AdminEmail,
                    UserName = dto.AdminEmail,
                    DisplayName = dto.DisplayName
                };

                var result = await _userManager.CreateAsync(adminUser, dto.Password);
                if (!result.Succeeded)
                    throw new BadRequestException(result.Errors.First().Description);

                await _userManager.AddToRoleAsync(adminUser, Roles.OrgAdmin);

                var uniqueKey = Guid.NewGuid().ToString("N")[..8].ToUpper();

                var organization = new Organization
                {
                    Name = dto.OrganizationName,
                    Description = dto.Description,
                    LogoUrl = dto.LogoUrl,
                    UniqueKey = uniqueKey,
                    Email = dto.AdminEmail
                };

                await _unitOfWork.Organizations.AddAsync(organization);
                await _unitOfWork.SaveChangesAsync();

                var member = new OrganizationMember
                {
                    UserId = adminUser.Id,
                    OrganizationId = organization.Id,
                    IsAdmin = true,
                    JoinedAt = DateTime.UtcNow
                };

                await _unitOfWork.OrganizationMembers.AddAsync(member);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _messageBus.PublishAsync("organization-created", new OrganizationCreatedEvent
                {
                    OrganizationId = organization.Id,
                    OrganizationName = organization.Name,
                    AdminEmail = adminUser.Email!,
                    AdminFullName = $"{adminUser.FirstName} {adminUser.LastName}",
                    UniqueKey = uniqueKey
                });

                return ApiResponse<CreatedOrganizationDto>.SuccessResponse(new CreatedOrganizationDto
                {
                    Id = organization.Id,
                    OrganizationName = organization.Name,
                    Description = organization.Description,
                    LogoUrl = organization.LogoUrl,
                    UniqueKey = uniqueKey,
                    AdminDisplayName = adminUser.DisplayName,
                    AdminEmail = adminUser.Email!,
                    CreatedAt = organization.CreatedAt
                }, "Organization registered successfully");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new NotFoundException("Invalid email or password");

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordCorrect)
                throw new UnauthorizedException("Invalid email or password");

            if (user.IsDeleted)
                throw new UnauthorizedException("Account has been deactivated");

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _tokenService.GenerateAccessToken(user);

            return ApiResponse<AuthResponseDto>.SuccessResponse(new AuthResponseDto
            {
                AccessToken = token,
                Email = user.Email!,
                FirstName = $"{user.FirstName} {user.LastName}",
                LastName = $"{user.FirstName} {user.LastName}",
                UserId = user.Id,
                Roles = roles.ToList()
            }, "Login successful");
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new NotFoundException("User not found");

            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new BadRequestException("Passwords do not match");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.First().Description);

            user.TokenVersion++;
            await _userManager.UpdateAsync(user);

            await _messageBus.PublishAsync("password-changed", new PasswordChangedEvent
            {
                UserId = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName = user.DisplayName ?? $"{user.FirstName} {user.LastName}"
            });

            return ApiResponse<bool>.SuccessResponse(true, "Password changed successfully");
        }

        public async Task<ApiResponse<bool>> LogoutAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new NotFoundException("User not found");

            user.TokenVersion++;
            await _userManager.UpdateAsync(user);

            return ApiResponse<bool>.SuccessResponse(true, "Logout successful");
        }
      
    }
}
