using VoteMe.Domain.Enum;

namespace VoteMe.Application.Helpers;

public static class PermissionChecker
{
    public static bool HasPermission(OrganizationRole orgRole, Permission permission)
    {
        return RolePermissions.OrganizationMap.TryGetValue(orgRole, out var perms) &&
               perms.Contains(permission);
    }

    public static bool HasPermission(bool isSuperAdmin, Permission permission)
    {
        if (isSuperAdmin)
            return true;

        return false;
    }

    // Most useful overload - combine both
    public static bool HasPermission(bool isSuperAdmin, OrganizationRole orgRole, Permission permission)
    {
        if (isSuperAdmin)
            return true;

        return RolePermissions.OrganizationMap.TryGetValue(orgRole, out var perms) &&
               perms.Contains(permission);
    }
}