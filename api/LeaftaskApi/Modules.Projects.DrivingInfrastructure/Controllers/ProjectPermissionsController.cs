using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Mvc;
using Modules.Projects.Application.Permissions.CreateRole;
using Modules.Projects.Application.Permissions.DeleteRole;
using Modules.Projects.Application.Permissions.GetPermissions;
using Modules.Projects.Application.Permissions.GetRoles;
using Modules.Projects.Application.Permissions.PatchRole;
using Modules.Projects.DrivingInfrastructure.Models.Requests;

namespace Modules.Projects.DrivingInfrastructure.Controllers;

[Route("api/v1/projects")]
public sealed class ProjectPermissionsController : ApiBaseController
{
    [HttpGet("{projectId:guid}/permissions")]
    public async Task<IActionResult> GetPermissions(
        [FromRoute] Guid projectId,
        CancellationToken cancellationToken = default)
    {
        GetProjectPermissionsQuery query = new(projectId);

        Result<IReadOnlyList<ProjectPermissionDto>> result = await Sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{projectId:guid}/roles")]
    public async Task<IActionResult> GetRoles(
        [FromRoute] Guid projectId,
        CancellationToken cancellationToken = default)
    {
        GetProjectRolesQuery query = new(projectId);

        Result<IReadOnlyList<ProjectRoleDto>> result = await Sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{projectId:guid}/roles")]
    public async Task<IActionResult> CreateRole(
        [FromRoute] Guid projectId,
        [FromBody] CreateProjectRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        CreateProjectRoleCommand command = new(
            projectId,
            request.Name,
            request.Permissions.Select(p => new CreateProjectRolePermissionInput(p.Id, p.Level)).ToList());

        Result<ProjectRoleDto> result = await Sender.Send(command, cancellationToken);

        return HandleResult(result, 201);
    }

    [HttpPatch("{projectId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> PatchRole(
        [FromRoute] Guid projectId,
        [FromRoute] Guid roleId,
        [FromBody] PatchProjectRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        PatchProjectRoleCommand command = new(
            projectId,
            roleId,
            request.Name,
            request.Permissions?.Select(p => new PatchProjectRolePermissionInput(p.Id, p.Level)).ToList());

        Result<ProjectRoleDto> result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{projectId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> DeleteRole(
        [FromRoute] Guid projectId,
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken = default)
    {
        DeleteProjectRoleCommand command = new(projectId, roleId);

        Result result = await Sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}
