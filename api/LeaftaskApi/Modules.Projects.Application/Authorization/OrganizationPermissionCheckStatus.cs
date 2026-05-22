namespace Modules.Projects.Application.Authorization;

public enum OrganizationPermissionCheckStatus
{
    Full = 0,
    Supervised = 1,
    Denied = 2,
    MembershipRequired = 3,
    PermissionNotFound = 4,
    OrganizationNotFound = 5
}
