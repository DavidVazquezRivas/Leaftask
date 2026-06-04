using System.ComponentModel;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.Organizations.Application.Roles.GetPermissions;
using Modules.Organizations.Application.Roles.GetRoles;

namespace Modules.Organizations.DrivingInfrastructure.Tools;

public class OrganizationRolesAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetOrganizationRoles")]
    [Description(
        "Retrieves a list of all roles defined within a specific organization. Useful for viewing existing roles or finding a specific role ID to assign to a user.")]
    public async Task<string> GetOrganizationRolesAsync(
        [Description("The unique identifier (GUID) of the organization.")]
        Guid orgId,
        CancellationToken cancellationToken = default)
    {
        GetOrganizationRolesQuery query = new(orgId);

        Result<IReadOnlyList<OrganizationRoleDto>> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetOrganizationRolesAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("GetAvailableOrganizationPermissions")]
    [Description(
        "Retrieves a list of all available system permissions that can be assigned to an organization role. Useful to know the valid permission IDs and levels before creating or modifying a role.")]
    public async Task<string> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        GetOrganizationPermissionsQuery query = new();

        Result<IReadOnlyList<OrganizationPermissionDto>> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetPermissionsAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }
}
