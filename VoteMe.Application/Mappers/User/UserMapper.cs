using VoteMe.Application.DTOs.User;
using VoteMe.Domain.Entities;

namespace VoteMe.Application.Mappers.User
{
    public static class UserMapper
    {
        public static UserDto ToDto(AppUser user, IList<string> roles)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt
            };
        }

        public static IEnumerable<UserDto> ToDtoList(IEnumerable<AppUser> users)
        {
            return users.Select(u => ToDto(u, new List<string>()));
        }
    }
}