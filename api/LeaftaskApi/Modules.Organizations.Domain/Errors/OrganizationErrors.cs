using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Domain.Errors;

public static class OrganizationErrors
{
    public static readonly Error OrganizationNotFound =
        new("Organization.NotFound", "The specified organization was not found.", 404);

    public static readonly Error OrganizationPermissionNotFound =
        new("Organization.Permission.NotFound", "The specified organization permission was not found.", 404);

    public static readonly Error OrganizationPermissionDenied =
        new("Organization.Permission.Denied", "You do not have permission to perform this operation.", 403);

    public static readonly Error OrganizationPermissionApprovalRequired =
        new("Organization.Permission.ApprovalRequired", "The action requires approval before it can be executed.", 403);

    public static readonly Error OrganizationMembershipRequired =
        new("Organization.Membership.Required", "You must be a member of the organization to perform this operation.", 403);

    public static readonly Error OrganizationMemberNotFound =
        new("Organization.Member.NotFound", "The specified organization member was not found.", 404);

    public static readonly Error OrganizationInvitationNotFound =
        new("Organization.Invitation.NotFound", "The specified organization invitation was not found.", 404);

    public static readonly Error InvalidInvitationStatus =
        new("Organization.Invitation.InvalidStatus", "The invitation status is invalid for this operation.", 409);

    public static readonly Error OrganizationRoleNotFound =
        new("Organization.Role.NotFound", "The specified organization role was not found.", 404);

    public static readonly Error OrganizationRolePermissionNotFound =
        new("Organization.RolePermission.NotFound", "The specified role permission was not found.", 404);
}
