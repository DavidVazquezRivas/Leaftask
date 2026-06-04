using System.ComponentModel;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.Projects.Application.Permissions.CreateRole;
using Modules.Projects.Application.Permissions.DeleteRole;
using Modules.Projects.Application.Permissions.GetPermissions;
using Modules.Projects.Application.Permissions.GetRoles;
using Modules.Projects.Application.Permissions.PatchRole;
using Modules.Projects.DrivingInfrastructure.Models.Requests;

namespace Modules.Projects.DrivingInfrastructure.Tools;

public class ProjectPermissionsAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetProjectAvailablePermissions")]
    [Description(
        "Retrieves a list of all available system permissions that can be assigned to a project role. Useful to know the valid permission IDs and levels before creating or modifying a role.")]
    public async Task<string> GetPermissionsAsync(
        [Description(
            "The unique identifier (GUID) of the project. If you only have the project name, resolve it first using 'GetOrganizationProjects'.")]
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        GetProjectPermissionsQuery query = new(projectId);

        Result<IReadOnlyList<ProjectPermissionDto>> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetPermissionsAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("GetProjectRoles")]
    [Description(
        "Retrieves a list of all roles defined within a specific project. Useful for viewing existing roles or finding a specific role ID to assign to a project member or to update/delete a role.")]
    public async Task<string> GetRolesAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        GetProjectRolesQuery query = new(projectId);

        Result<IReadOnlyList<ProjectRoleDto>> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetRolesAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("CreateProjectRole")]
    [Description("Creates a new custom role within a project and assigns it specific permissions.")]
    public async Task<string> CreateRoleAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The name of the new role (e.g., 'Senior Developer', 'Guest').")]
        string name,
        [Description(
            "The list of permissions to assign. Each item must have an 'Id' (GUID) and a 'Level' (0 = None, 1 = Supervised, 2 = Full). You MUST use 'GetProjectAvailablePermissions' first to know valid IDs.")]
#pragma warning disable CA1002 // Do not expose generic lists
        List<CreateProjectRolePermissionRequest> permissions,
#pragma warning restore CA1002 // Do not expose generic lists
        CancellationToken cancellationToken = default)
    {
        CreateProjectRoleCommand command = new(
            projectId,
            name,
            permissions?.Select(p => new CreateProjectRolePermissionInput(p.Id, p.Level)).ToList() ?? []);

        Result<ProjectRoleDto> result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(CreateRoleAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("UpdateProjectRole")]
    [Description("Updates an existing project role's name or its associated permissions.")]
    public async Task<string> PatchRoleAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the role to update. Find this ID first using 'GetProjectRoles'.")]
        Guid roleId,
        [Description("The new name for the role. Leave as null if you don't want to change the current name.")]
        string? name = null,
        [Description(
            "The updated list of permissions. Each item must have an 'Id' (GUID) and a 'Level' (0 = None, 1 = Supervised, 2 = Full). Leave as null if you don't want to change current permissions.")]
#pragma warning disable CA1002 // Do not expose generic lists
        List<CreateProjectRolePermissionRequest>? permissions = null,
#pragma warning restore CA1002 // Do not expose generic lists
        CancellationToken cancellationToken = default)
    {
        PatchProjectRoleCommand command = new(
            projectId,
            roleId,
            name,
            permissions?.Select(p => new PatchProjectRolePermissionInput(p.Id, p.Level)).ToList());

        Result<ProjectRoleDto> result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(PatchRoleAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("DeleteProjectRole")]
    [Description("Deletes a custom role from a project. Use carefully.")]
    public async Task<string> DeleteRoleAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the role to delete. Find this ID first using 'GetProjectRoles'.")]
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        DeleteProjectRoleCommand command = new(projectId, roleId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(DeleteRoleAsync), result.Error.Description);
        }

        return formatter.FormatMessage("Project role deleted successfully.");
    }
}
