using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using BuildingBlocks.DrivingInfrastructure.Responses.Meta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.WorkItems.Application.WorkLogs;
using Modules.WorkItems.Application.WorkLogs.Create;
using Modules.WorkItems.Application.WorkLogs.Delete;
using Modules.WorkItems.Application.WorkLogs.List;
using Modules.WorkItems.Application.WorkLogs.Update;
using Modules.WorkItems.DrivingInfrastructure.Models.Requests;

namespace Modules.WorkItems.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/workitems/{projectId:guid}/{itemId:guid}/work-logs")]
public sealed class WorkItemTimeTrackingController : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetWorkLogs(
        Guid projectId,
        Guid itemId,
        [FromQuery] int limit = 10,
        [FromQuery] string? cursor = null,
        [FromQuery] string[]? sort = null,
        CancellationToken cancellationToken = default)
    {
        Result<PaginatedResult<WorkLogDto>> result = await Sender.Send(
            new GetWorkLogsQuery(projectId, itemId, limit, cursor, sort ?? []),
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

    [HttpPost]
    public async Task<IActionResult> LogWork(
        Guid projectId,
        Guid itemId,
        [FromBody] LogWorkRequest request,
        CancellationToken cancellationToken = default) =>
        HandleResult(
            await Sender.Send(
                new LogWorkCommand(projectId, itemId, request.Dedication, request.Date, request.Description),
                cancellationToken),
            201);

    [HttpPatch("{logId:guid}")]
    public async Task<IActionResult> UpdateWorkLog(
        Guid projectId,
        Guid itemId,
        Guid logId,
        [FromBody] UpdateWorkLogRequest request,
        CancellationToken cancellationToken = default) =>
        HandleResult(await Sender.Send(
            new UpdateWorkLogCommand(projectId, itemId, logId, request.Dedication, request.Date, request.Description),
            cancellationToken));

    [HttpDelete("{logId:guid}")]
    public async Task<IActionResult> DeleteWorkLog(
        Guid projectId,
        Guid itemId,
        Guid logId,
        CancellationToken cancellationToken = default)
    {
        Result result = await Sender.Send(new DeleteWorkLogCommand(projectId, itemId, logId), cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }
}
