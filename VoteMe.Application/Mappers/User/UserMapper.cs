using VoteMe.Application.DTOs.User;
using VoteMe.Domain.Entities;

namespace VoteMe.Application.Mappers.User
{
    public static class UserMapper
    {
        public static OrganizationUserDto ToOrgUserDto(AppUser user,OrganizationMember member)
        {
            return new OrganizationUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = member.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public static IEnumerable<OrganizationUserDto> ToOrgUserDtoList(IEnumerable<AppUser> users,OrganizationMember member)
        {
            return users.Select(u => ToOrgUserDto(u, member));
        }
        public static UserDto ToUserDto(AppUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                CreatedAt = user.CreatedAt
            };
        }

        public static IEnumerable<UserDto> ToUserDtoList(IEnumerable<AppUser> users)
        {
            return users.Select(u => ToUserDto(u));
        }
    }
}