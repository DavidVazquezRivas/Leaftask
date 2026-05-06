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
}
