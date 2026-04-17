using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Domain.Errors;

public static class OrganizationErrors
{
    public static readonly Error OrganizationNotFound =
        new("Organization.NotFound", "The specified organization was not found.", 404);

    public static readonly Error InvalidInvitationStatus =
        new("Organization.Invitation.InvalidStatus", "The invitation status is invalid for this operation.", 409);
}
