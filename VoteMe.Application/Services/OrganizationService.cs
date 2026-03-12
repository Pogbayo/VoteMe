using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VoteMe.Application.Authorization;
using VoteMe.Application.Common;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.Events.Organization;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Application.Mappers.Organization;
using VoteMe.Domain.Constants;
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
        private readonly IImageService _imageService;
        public OrganizationService(IUnitOfWork unitOfWork,IImageService imageService,UserManager<AppUser> userManager, ICurrentUserService currentUserService, IMessageBus messageBus, ICacheService cacheService, ILogger<OrganizationService> logger)
        {
            _logger = logger;
            _imageService = imageService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _messageBus = messageBus;
            _cacheService = cacheService;
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

        public async Task<ApiResponse<CreatedOrganizationDto>> CreateOrganizationAsync(
             CreateOrganizationDto dto,
             IFormFile? logoFile)
        {
            // 1. input validation
            if (string.IsNullOrWhiteSpace(dto.OrganizationName))
                throw new BadRequestException("Organization name is required");

            if (string.IsNullOrWhiteSpace(dto.AdminEmail))
                throw new BadRequestException("Admin email is required");

            var trimmedEmail = dto.AdminEmail.Trim();

            // Begin transaction
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 2. Find or create admin user (Identity operations are outside EF transaction)
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

                // 3. Generate unique key
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

                // 4. Create organization 
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

                // 5. Upload logo 
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

                // 6. Create membership
                var membership = new OrganizationMember
                {
                    UserId = adminUser.Id,
                    OrganizationId = organization.Id,
                    IsAdmin = true,
                    JoinedAt = DateTime.UtcNow
                };

                await _unitOfWork.OrganizationMembers.AddAsync(membership);
                await _unitOfWork.SaveChangesAsync();

                // 7. Publish event (after successful save)
                await _messageBus.PublishAsync("organization-created", new OrganizationCreatedEvent
                {
                    AdminUserId = adminUser.Id,
                    AdminEmail = adminUser.Email ?? string.Empty,
                    AdminDisplayName = adminUser.DisplayName ?? string.Empty,
                    OrganizationName = organization.Name,
                    UniqueKey = organization.UniqueKey
                });

                // 8. Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                // 9. Prepare response
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

        public Task<ApiResponse<OrganizationDto>> GetOrganizationAsync(Guid organizationId)
        {
            //var org = await _unitOfWork.Organizations.FindOneAsync()
            throw new NotImplementedException();

        }

        public void UpdateOrganizationAsync(Guid organizationId, UpdateOrganizationDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
