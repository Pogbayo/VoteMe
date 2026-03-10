using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Domain.Exceptions;

namespace VoteMe.Application.Authorization
{
    public static class OrganizationAuthorization
    {
        public static async Task RequireCurrentUserIsOrgAdmin(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            Guid organizationId,
            string action = "perform this action")
        {
            var membership = await unitOfWork.OrganizationMembers
                .FindOneAsync(m =>
                    m.UserId == currentUserService.UserId &&
                    m.OrganizationId == organizationId);

            if (membership is null)
            {
                throw new ForbiddenException("You are not a member of this organization.");
            }

            if (!membership.IsAdmin)
            {
                throw new ForbiddenException($"Only organization administrators can {action}.");
            }
        }
    }
}