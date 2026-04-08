using VoteMe.Domain.Enum;

namespace VoteMe.Application.Helpers;

public static class RolePermissions
{
    public static readonly Dictionary<OrganizationRole, HashSet<Permission>> OrganizationMap = new()
    {
        {
            OrganizationRole.Owner, new HashSet<Permission>
            {
                Permission.CreateOrganization,
                Permission.UpdateOrganization,
                Permission.DeleteOrganization,
                Permission.CreateElection,
                Permission.UpdateElection,
                Permission.DeleteElection,
                Permission.OpenElection,
                Permission.CloseElection,
                Permission.CreateElectionCategory,
                Permission.UpdateElectionCategory,
                Permission.DeleteElectionCategory,
                Permission.CreateCandidate,
                Permission.UpdateCandidate,
                Permission.DeleteCandidate,
                Permission.Vote,
                Permission.ApproveMember,
                Permission.RemoveMember,
                Permission.PromoteToAdmin,
                Permission.DemoteFromAdmin,
                Permission.ViewMembers
            }
        },
        {
            OrganizationRole.Admin, new HashSet<Permission>
            {
                Permission.CreateElection,
                Permission.UpdateElection,
                Permission.CreateElectionCategory,
                Permission.UpdateElectionCategory,
                Permission.CreateCandidate,
                Permission.UpdateCandidate,
                Permission.DeleteCandidate,
                Permission.Vote,
                Permission.ApproveMember,
                Permission.RemoveMember,
                Permission.ViewMembers
            }
        },
        {
            OrganizationRole.Member, new HashSet<Permission>
            {
                Permission.Vote
            }
        }
    };

    public static HashSet<Permission> SuperAdminPermissions { get; } =
        new HashSet<Permission>(Enum.GetValues<Permission>());
}