using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Domain.Errors;

public static class OrganizationErrors
{
    public static readonly Error OrganizationNotFound =
        new("Organization.NotFound", "The specified organization was not found.", 404);
}
