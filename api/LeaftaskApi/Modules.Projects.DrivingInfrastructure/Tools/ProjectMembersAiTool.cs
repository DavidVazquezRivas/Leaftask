using System.ComponentModel;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.Projects.Application.Members.GetCount;
using Modules.Projects.Application.Members.GetMembers;
using Modules.Projects.Application.Members.Remove;
using Modules.Projects.Application.Members.UpdateRole;

namespace Modules.Projects.DrivingInfrastructure.Tools;

public class ProjectMembersAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetProjectMemberCount")]
    [Description("Retrieves the total number of members currently assigned to a specific project.")]
    public async Task<string> GetMemberCountAsync(
        [Description(
            "The unique identifier (GUID) of the project. If you only have the project name, resolve it first using 'GetOrganizationProjects' or 'GetMyProjects'.")]
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        Result<ProjectMemberCountDto> result =
            await sender.Send(new GetProjectMemberCountQuery(projectId), cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetMemberCountAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("GetProjectMembers")]
    [Description(
        "Retrieves a list of members belonging to a specific project. Useful for finding who is working on Leaftask projects and checking their current roles.")]
    public async Task<string> GetMembersAsync(
        [Description(
            "The unique identifier (GUID) of the project. If you only have the project name, resolve it first using 'GetOrganizationProjects' or 'GetMyProjects'.")]
        Guid projectId,
        [Description("Max results. Keep it low to avoid context overload. Default is 10.")]
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        GetProjectMembersQuery query = new()
        {
            ProjectId = projectId,
            Limit = limit,
            Cursor = null,
            Sort = []
        };

        Result<PaginatedResult<ProjectMemberDto>> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetMembersAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value.Items);
    }

    [KernelFunction("UpdateProjectMemberRole")]
    [Description(
        "Updates the role of an existing member within a project. Use this to promote, demote, or modify team member access.")]
    public async Task<string> UpdateMemberRoleAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description(
            "The unique identifier (GUID) of the member whose role needs to change. Do NOT pass a global User ID. Find this ID first using 'GetProjectMembers'.")]
        Guid memberId,
        [Description(
            "The unique identifier (GUID) of the new role. Resolve semantic names to a GUID using 'GetOrganizationRoles' before calling this tool.")]
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        UpdateProjectMemberRoleCommand command = new(projectId, memberId, roleId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(UpdateMemberRoleAsync), result.Error.Description);
        }

        return formatter.FormatMessage("Project member role updated successfully.");
    }

    [KernelFunction("RemoveProjectMember")]
    [Description(
        "Removes a member from a project. Use this carefully when someone should no longer have access or involvement in the project.")]
    public async Task<string> RemoveMemberAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description(
            "The unique identifier (GUID) of the member to be removed. Do NOT pass a global User ID. Find this ID first using 'GetProjectMembers'.")]
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        RemoveProjectMemberCommand command = new(projectId, memberId);

        Result result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(RemoveMemberAsync), result.Error.Description);
        }

        return formatter.FormatMessage("Project member removed successfully.");
    }
}
