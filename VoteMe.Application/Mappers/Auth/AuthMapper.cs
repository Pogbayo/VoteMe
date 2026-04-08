using VoteMe.Application.DTOs.Auth;
using VoteMe.Application.DTOs.Organization;
using VoteMe.Domain.Entities;

namespace VoteMe.Application.Mappers.Auth
{
    public static class AuthMapper
    {
        public static AuthResponseDto ToAuthResponseDto(AppUser user, string token)
        {
            return new AuthResponseDto
            {
                AccessToken = token,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserId = user.Id,
            };
        }

    }
}