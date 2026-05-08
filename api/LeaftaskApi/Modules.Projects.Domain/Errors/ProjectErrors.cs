using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Domain.Errors;

public static class ProjectErrors
{
    public static readonly Error ProjectNotFound =
        new("Project.NotFound", "The specified project was not found.", 404);

    public static readonly Error DuplicatedAbbreviation =
        new("Project.Abbreviation.Duplicated", "A project with the same abbreviation already exists.", 400);

    public static readonly Error InvalidPrivacyLevel =
        new("Project.Privacy.Invalid", "The specified privacy level is invalid.", 400);

    public static readonly Error OwnerNotFound =
        new("Project.Owner.NotFound", "The specified project owner was not found.", 404);

    public static readonly Error OrganizationNotFound =
        new("Project.Organization.NotFound", "The specified organization was not found.", 404);

    public static readonly Error OrganizationPermissionNotFound =
        new("Project.Organization.Permission.NotFound", "The specified organization permission was not found.", 404);

    public static readonly Error OrganizationPermissionDenied =
        new("Project.Organization.Permission.Denied", "You do not have permission to perform this operation.", 403);

    public static readonly Error OrganizationPermissionApprovalRequired =
        new("Project.Organization.Permission.ApprovalRequired", "The action requires approval before it can be executed.", 403);

    public static readonly Error OrganizationMembershipRequired =
        new("Project.Organization.Membership.Required", "You must be a member of the organization to perform this operation.", 403);

    public static readonly Error AccessDenied =
        new("Project.AccessDenied", "You do not have access to this project.", 403);

    public static readonly Error RoleNotFound =
        new("Project.Role.NotFound", "The specified role was not found in this project.", 404);

    public static readonly Error DuplicatedRoleName =
        new("Project.Role.Name.Duplicated", "A role with the same name already exists in this project.", 409);

    public static readonly Error PermissionNotFound =
        new("Project.Permission.NotFound", "The specified permission was not found.", 404);

    public static readonly Error InvitationNotFound =
        new("Project.Invitation.NotFound", "The specified invitation was not found.", 404);

    public static readonly Error InvalidInvitationStatus =
        new("Project.Invitation.InvalidStatus", "The invitation is not in the expected status.", 409);

    public static readonly Error InvitationAccessDenied =
        new("Project.Invitation.AccessDenied", "You cannot act on this invitation.", 403);

    public static readonly Error MemberNotFound =
        new("Project.Member.NotFound", "The specified member was not found in this project.", 404);

    public static readonly Error UserAlreadyMember =
        new("Project.Member.AlreadyMember", "The user is already a member of this project.", 400);

    public static readonly Error UserAlreadyInvited =
        new("Project.Invitation.AlreadyPending", "The user already has a pending invitation for this project.", 400);

    public static readonly Error OwnerRoleCannotBeDeleted =
        new("Project.Role.Owner.CannotBeDeleted", "The owner role cannot be deleted.", 403);

    public static readonly Error OwnerRoleCannotBeModified =
        new("Project.Role.Owner.CannotBeModified", "The owner role permissions cannot be modified.", 403);

    public static readonly Error FieldTypeNotFound =
        new("Project.Field.TypeNotFound", "The specified field type was not found.", 404);

    public static readonly Error OptionsRequired =
        new("Project.Field.OptionsRequired", "Options are required for selection field types.", 400);

    public static readonly Error CustomFieldNotFound =
        new("Project.Field.NotFound", "The specified custom field was not found in this project.", 404);
}
