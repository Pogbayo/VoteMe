using VoteMe.Application.DTOs.Auth;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Domain.Entities;

namespace VoteMe.Application.Mappers.Auth
{
    public static class AuthMapper
    {
        public static AuthResponseDto ToAuthResponseDto(AppUser user, string token, IList<string> roles)
        {
            return new AuthResponseDto
            {
                AccessToken = token,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                UserId = user.Id,
                Roles = roles.ToList()
            };
        }

        public static CreatedOrganizationDto ToCreatedOrganizationDto(Domain.Entities.Organization organization, AppUser admin, string uniqueKey)
        {
            return new CreatedOrganizationDto
            {
                Id = organization.Id,
                OrganizationName = organization.Name,
                Description = organization.Description,
                LogoUrl = organization.LogoUrl,
                UniqueKey = uniqueKey,
                AdminFirstName = admin.FirstName,
                AdminLastName = admin.LastName,
                AdminEmail = admin.Email!,
                CreatedAt = organization.CreatedAt
            };
        }
    }
}