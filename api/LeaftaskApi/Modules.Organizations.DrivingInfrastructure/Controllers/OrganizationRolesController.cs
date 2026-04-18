using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Mvc;
using Modules.Organizations.Application.Roles.Create;
using Modules.Organizations.Application.Roles.Delete;
using Modules.Organizations.Application.Roles.GetPermissions;
using Modules.Organizations.Application.Roles.GetRoles;
using Modules.Organizations.Application.Roles.Patch;
using Modules.Organizations.DrivingInfrastructure.Models.Requests;

namespace Modules.Organizations.DrivingInfrastructure.Controllers;

[Route("api/v1/organizations")]
public class OrganizationRolesController : ApiBaseController
{
    [HttpPost("{orgId:guid}/roles")]
    public async Task<IActionResult> CreateRole(
        [FromRoute] Guid orgId,
        [FromBody] CreateOrganizationRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        CreateOrganizationRoleCommand command = new(
            orgId,
            request.Name,
            request.Permissions?.Select(permission => new CreateOrganizationRolePermissionInput(permission.Id, permission.Level)).ToList() ?? []);

        Result<OrganizationRoleResponse> result = await Sender.Send(command, cancellationToken);

        return HandleResult(result, 201);
    }

    [HttpPatch("{orgId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> PatchRole(
        [FromRoute] Guid orgId,
        [FromRoute] Guid roleId,
        [FromBody] PatchOrganizationRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        PatchOrganizationRoleCommand command = new(
            orgId,
            roleId,
            request.Name,
            request.Permissions?.Select(permission => new PatchOrganizationRolePermissionInput(permission.Id, permission.Level)).ToList());

        Result<OrganizationRoleResponse> result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{orgId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> DeleteRole(
        [FromRoute] Guid orgId,
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken = default)
    {
        DeleteOrganizationRoleCommand command = new(orgId, roleId);

        Result result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{orgId:guid}/roles")]
    public async Task<IActionResult> GetRoles(
        [FromRoute] Guid orgId,
        CancellationToken cancellationToken = default)
    {
        GetOrganizationRolesQuery query = new(orgId);

        Result<IReadOnlyList<OrganizationRoleDto>> result = await Sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions(CancellationToken cancellationToken = default)
    {
        GetOrganizationPermissionsQuery query = new();

        Result<IReadOnlyList<OrganizationPermissionDto>> result = await Sender.Send(query, cancellationToken);

        return HandleResult(result);
    }
}
