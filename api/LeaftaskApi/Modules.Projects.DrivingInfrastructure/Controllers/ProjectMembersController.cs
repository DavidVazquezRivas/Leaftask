using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Projects.Application.Members.GetCount;
using Modules.Projects.Application.Members.GetMembers;
using Modules.Projects.Application.Members.Remove;
using Modules.Projects.Application.Members.UpdateRole;
using Modules.Projects.DrivingInfrastructure.Models.Requests;

namespace Modules.Projects.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/projects")]
public sealed class ProjectMembersController : ApiBaseController
{
    [HttpGet("{projectId:guid}/members/count")]
    public async Task<IActionResult> GetMemberCount(Guid projectId, CancellationToken cancellationToken) =>
        HandleResult(await Sender.Send(new GetProjectMemberCountQuery(projectId), cancellationToken));

    [HttpGet("{projectId:guid}/members")]
    public async Task<IActionResult> GetMembers(
        Guid projectId,
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] IReadOnlyCollection<string>? sort = null,
        CancellationToken cancellationToken = default)
    {
        GetProjectMembersQuery query = new()
        {
            ProjectId = projectId,
            Limit = limit,
            Cursor = cursor,
            Sort = sort ?? []
        };

        Result<PaginatedResult<ProjectMemberDto>> result = await Sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        IReadOnlyList<SortMeta>? sortMeta = SortMetaParser.Parse(sort);
        PaginationMeta paginationMeta = new()
        {
            Limit = limit,
            NextCursor = result.Value.NextCursor,
            HasMore = result.Value.HasMore
        };

        return StatusCode(200, BuildSuccessResponse(result.Value.Items, sortMeta, paginationMeta));
    }

    [HttpPatch("{projectId:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> UpdateMemberRole(
        Guid projectId,
        Guid memberId,
        [FromBody] UpdateProjectMemberRoleRequest request,
        CancellationToken cancellationToken) =>
        HandleResult(await Sender.Send(
            new UpdateProjectMemberRoleCommand(projectId, memberId, request.RoleId), cancellationToken));

    [HttpDelete("{projectId:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> RemoveMember(
        Guid projectId,
        Guid memberId,
        CancellationToken cancellationToken) =>
        HandleResult(await Sender.Send(new RemoveProjectMemberCommand(projectId, memberId), cancellationToken));
}
