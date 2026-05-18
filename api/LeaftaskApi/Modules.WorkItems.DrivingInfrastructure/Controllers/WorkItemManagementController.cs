using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.WorkItems.Application.WorkItems.Create;
using Modules.WorkItems.Application.WorkItems.Delete;
using Modules.WorkItems.Application.WorkItems.GetProjectWorkItems;
using Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;
using Modules.WorkItems.Application.WorkItems.Update;
using Modules.WorkItems.DrivingInfrastructure.Models.Requests;

namespace Modules.WorkItems.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/workitems")]
public sealed class WorkItemManagementController : ApiBaseController
{
    [HttpGet("{projectId:guid}")]
    public async Task<IActionResult> GetProjectWorkItems(
        Guid projectId,
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] string[]? sort = null,
        CancellationToken cancellationToken = default)
    {
        Result<PaginatedResult<WorkItemListDto>> result = await Sender.Send(
            new GetProjectWorkItemsQuery
            {
                ProjectId = projectId,
                Limit = limit,
                Cursor = cursor,
                Sort = sort ?? []
            },
            cancellationToken);

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

    [HttpGet("{projectId:guid}/{itemId:guid}")]
    public async Task<IActionResult> GetWorkItemDetails(
        Guid projectId,
        Guid itemId,
        CancellationToken cancellationToken = default) =>
        HandleResult(await Sender.Send(
            new GetWorkItemDetailsQuery(projectId, itemId),
            cancellationToken));

    [HttpDelete("{projectId:guid}/{itemId:guid}")]
    public async Task<IActionResult> DeleteWorkItem(
        Guid projectId,
        Guid itemId,
        CancellationToken cancellationToken = default) =>
        HandleResult(await Sender.Send(
            new DeleteWorkItemCommand(projectId, itemId),
            cancellationToken));

    [HttpPatch("{projectId:guid}/{itemId:guid}")]
    public async Task<IActionResult> UpdateWorkItem(
        Guid projectId,
        Guid itemId,
        [FromBody] UpdateWorkItemRequest request,
        CancellationToken cancellationToken = default) =>
        HandleResult(await Sender.Send(
            new UpdateWorkItemCommand(
                projectId,
                itemId,
                request.Title,
                request.Description,
                request.StatusId,
                request.TypeId,
                request.AssigneeId,
                request.UpdateAssignee ?? false,
                request.Progress,
                request.Estimation,
                request.LimitDate,
                request.ParentId,
                request.UpdateParent ?? false,
                request.CustomFields),
            cancellationToken));

    [HttpPost("{projectId:guid}")]
    public async Task<IActionResult> CreateWorkItem(
        Guid projectId,
        [FromBody] CreateWorkItemRequest request,
        CancellationToken cancellationToken = default) =>
        HandleResult(
            await Sender.Send(
                new CreateWorkItemCommand(
                    projectId,
                    request.Title,
                    request.Description,
                    request.Estimation,
                    request.TypeId,
                    request.StatusId,
                    request.AssigneeId,
                    request.ParentId),
                cancellationToken),
            201);
}
