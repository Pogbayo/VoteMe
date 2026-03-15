using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Auth;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.Events.Auth;
using VoteMe.Application.Events.Organization;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Application.Mappers.Auth;
using VoteMe.Application.Mappers.Organization;
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
        private readonly IImageService _imageService;
        private readonly ILogger<AuthService> _logger;


        public AuthService(
            IImageService imageService,
            ILogger<AuthService> logger,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IMessageBus messageBus,
            IUnitOfWork unitOfWork,
            ITokenService tokenService)
        {
            _imageService = imageService;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _messageBus = messageBus;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }


        private string GenerateUniqueKey(int length = 8)
        {
            const string chars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
            var bytes = Guid.NewGuid().ToByteArray();
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[(bytes[i % bytes.Length] ^ (i * 37)) % chars.Length];
            }

            return new string(result);
        }


        public async Task<ApiResponse<CreatedOrganizationDto>> RegisterOrganizationAsync(
             CreateOrganizationDto dto,
             IFormFile? logoFile)
        {
            if (string.IsNullOrWhiteSpace(dto.OrganizationName))
                throw new BadRequestException("Organization name is required");

            if (string.IsNullOrWhiteSpace(dto.AdminEmail))
                throw new BadRequestException("Admin email is required");

            var trimmedEmail = dto.AdminEmail.Trim();

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(trimmedEmail);
                AppUser adminUser;

                if (existingUser == null)
                {
                    adminUser = new AppUser
                    {
                        UserName = trimmedEmail,
                        Email = trimmedEmail,
                        FirstName = dto.AdminFirstName?.Trim() ?? string.Empty,
                        LastName = dto.AdminLastName?.Trim() ?? string.Empty,
                        DisplayName = dto.AdminDisplayName?.Trim() ?? string.Empty,
                        EmailConfirmed = false,
                        TokenVersion = 1,
                        CreatedAt = DateTime.UtcNow
                    };

                    var createResult = await _userManager.CreateAsync(adminUser, dto.Password);
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        throw new BadRequestException($"Failed to create admin account: {errors}");
                    }

                    await _userManager.AddToRoleAsync(adminUser, Roles.OrgAdmin);
                }
                else
                {
                    adminUser = existingUser;

                    if (!await _userManager.IsInRoleAsync(adminUser, Roles.OrgAdmin))
                    {
                        await _userManager.AddToRoleAsync(adminUser, Roles.OrgAdmin);
                    }
                }

                const int maxAttempts = 10;
                string uniqueKey;
                int attempts = 0;

                do
                {
                    if (attempts++ >= maxAttempts)
                    {
                        throw new Domain.Exceptions.InvalidOperationException(
                            "Unable to generate a unique organization key after multiple attempts. Please try again later.");
                    }

                    uniqueKey = GenerateUniqueKey();
                }
                while (await _unitOfWork.Organizations.ExistsAsync(o => o.UniqueKey == uniqueKey));

                var organization = new Organization
                {
                    Name = dto.OrganizationName.Trim(),
                    Description = dto.Description?.Trim() ?? string.Empty,
                    Email = trimmedEmail,
                    UniqueKey = uniqueKey,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Organizations.AddAsync(organization);
                await _unitOfWork.SaveChangesAsync();

                string? logoUrl = null;
                if (logoFile != null && logoFile.Length > 0)
                {
                    logoUrl = await _imageService.UploadImageAsync(
                        logoFile,
                        "Organization",
                        organization.Id); // folder: /Organization/{organization.Id}/logo

                    if (string.IsNullOrEmpty(logoUrl))
                    {
                        throw new Domain.Exceptions.InvalidOperationException("Failed to upload organization logo.");
                    }

                    organization.LogoUrl = logoUrl;
                    _unitOfWork.Organizations.Update(organization);
                    await _unitOfWork.SaveChangesAsync();
                }

                var membership = new OrganizationMember
                {
                    UserId = adminUser.Id,
                    OrganizationId = organization.Id,
                    IsAdmin = true,
                    JoinedAt = DateTime.UtcNow
                };

                await _unitOfWork.OrganizationMembers.AddAsync(membership);
                await _unitOfWork.SaveChangesAsync();

                await _messageBus.PublishAsync("organization-created", new OrganizationCreatedEvent
                {
                    AdminUserId = adminUser.Id,
                    AdminEmail = adminUser.Email ?? string.Empty,
                    AdminDisplayName = adminUser.DisplayName ?? string.Empty,
                    OrganizationName = organization.Name,
                    UniqueKey = organization.UniqueKey
                });

                await _unitOfWork.CommitTransactionAsync();

                var responseDto = OrganizationMapper.ToCreatedOrganizationDto(organization);

                return ApiResponse<CreatedOrganizationDto>.SuccessResponse(
                    responseDto,
                    "Organization created successfully. Share the UniqueKey with members to join.");
            }
            catch (Exception ex)
            {
                _logger.Log(
                    LogLevel.Warning,
                    new EventId(),
                    ex,
                    ex,
                    (state, exception) => exception?.Message ?? "Exception occurred"
                );
                await _unitOfWork.RollbackTransactionAsync();

                throw;
            }
        }


        public async Task<ApiResponse<AuthResponseDto>> RegisterUserAsync(RegisterUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequestException("Email is required");
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequestException("Password is required");
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new BadRequestException("First name is required");
            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new BadRequestException("Last name is required");
            if (string.IsNullOrWhiteSpace(dto.UniqueKey))
                throw new BadRequestException("Unique key is required");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                    throw new BadRequestException("Email already exists");

                var organization = await _unitOfWork.Organizations.GetByUniqueKeyAsync(dto.UniqueKey);
                if (organization == null)
                    throw new BadRequestException("Invalid unique key");

                var displayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? null : dto.DisplayName.Trim();

                var user = new AppUser
                {
                    FirstName = dto.FirstName.Trim(),
                    LastName = dto.LastName.Trim(),
                    DisplayName = displayName!,
                    Email = dto.Email.Trim().ToLower(),
                    UserName = dto.Email.Trim().ToLower()
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    throw new BadRequestException(result.Errors.First().Description);

                await _userManager.AddToRoleAsync(user, Roles.Voter);

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
                    Email = user.Email!,
                    DisplayName = user.DisplayName ?? $"{user.FirstName} {user.LastName}"
                });

                var roles = await _userManager.GetRolesAsync(user);
                var token = await _tokenService.GenerateAccessTokenAsync(user);

                return ApiResponse<AuthResponseDto>.SuccessResponse(
                    AuthMapper.ToAuthResponseDto(user, token, roles),
                    "Registration successful");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

      
        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequestException("Email is required");
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequestException("Password is required");

            var user = await _userManager.FindByEmailAsync(dto.Email.Trim().ToLower());
            if (user == null)
                throw new NotFoundException("Invalid email or password");

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordCorrect)
                throw new UnauthorizedException("Invalid email or password");

            if (user.IsDeleted)
                throw new UnauthorizedException("Account has been deactivated");

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _tokenService.GenerateAccessTokenAsync(user);

            return ApiResponse<AuthResponseDto>.SuccessResponse(
                AuthMapper.ToAuthResponseDto(user, token, roles),
                "Login successful");
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                throw new BadRequestException("Current password is required");
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                throw new BadRequestException("New password is required");
            if (string.IsNullOrWhiteSpace(dto.ConfirmNewPassword))
                throw new BadRequestException("Confirm password is required");
            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new BadRequestException("Passwords do not match");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new NotFoundException("User not found");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.First().Description);

            user.TokenVersion++;
            await _userManager.UpdateAsync(user);

            await _messageBus.PublishAsync("password-changed", new PasswordChangedEvent
            {
                UserId = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName ?? $"{user.FirstName} {user.LastName}"
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