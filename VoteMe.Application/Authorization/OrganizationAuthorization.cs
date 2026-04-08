using VoteMe.Application.Helpers;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Domain.Enum;
using VoteMe.Domain.Exceptions;

namespace VoteMe.Application.Authorization
{
    public static class OrganizationAuthorization
    {
        public static async Task RequirePermission(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            Guid organizationId,
            Permission permission,
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

            if (!PermissionChecker.HasPermission(membership.Role, permission))
            {
                throw new ForbiddenException($"You are not allowed to {action}.");
            }
        }
    }
}