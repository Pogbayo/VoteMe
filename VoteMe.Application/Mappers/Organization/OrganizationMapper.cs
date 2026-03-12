using VoteMe.Application.DTOs.Organization;
using VoteMe.Application.DTOs.OrganizationMember;
using VoteMe.Domain.Entities;

namespace VoteMe.Application.Mappers.Organization
{
    public static class OrganizationMapper
    {
        public static OrganizationDto ToDto(Domain.Entities.Organization organization)
        {
            return new OrganizationDto
            {
                Id = organization.Id,
                Name = organization.Name,
                Description = organization.Description,
                AdminEmail = organization.Email,
                LogoUrl = organization.LogoUrl,
                UniqueKey = organization.UniqueKey,
                IsActive = organization.IsActive,
                CreatedAt = organization.CreatedAt
            };
        }
        public static CreatedOrganizationDto ToCreatedOrganizationDto(Domain.Entities.Organization organization)
        {
            return new CreatedOrganizationDto
            {
                Id = organization.Id,
                OrganizationName = organization.Name,
                Description = organization.Description,
                AdminEmail = organization.Email,
                LogoUrl = organization.LogoUrl,
                UniqueKey = organization.UniqueKey,
                CreatedAt = organization.CreatedAt
            };
        }

        public static IEnumerable<OrganizationDto> ToDtoList(IEnumerable<Domain.Entities.Organization> organizations)
        {
            return organizations.Select(ToDto);
        }

        public static OrganizationMemberDto ToMemberDto(OrganizationMember member)
        {
            return new OrganizationMemberDto
            {
                UserId = member.UserId,
                FullName = $"{member.User.FirstName} {member.User.LastName}",
                Email = member.User.Email!,
                IsAdmin = member.IsAdmin,
                JoinedAt = member.JoinedAt
            };
        }

        public static IEnumerable<OrganizationMemberDto> ToMemberDtoList(IEnumerable<OrganizationMember> members)
        {
            return members.Select(ToMemberDto);
        }
    }
}