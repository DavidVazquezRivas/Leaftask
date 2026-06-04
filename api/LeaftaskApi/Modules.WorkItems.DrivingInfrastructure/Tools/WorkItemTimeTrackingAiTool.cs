using System.ComponentModel;
using System.Globalization;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.WorkItems.Application.WorkLogs;
using Modules.WorkItems.Application.WorkLogs.Create;
using Modules.WorkItems.Application.WorkLogs.Delete;
using Modules.WorkItems.Application.WorkLogs.List;
using Modules.WorkItems.Application.WorkLogs.Update;

namespace Modules.WorkItems.DrivingInfrastructure.Tools;

public class WorkItemTimeTrackingAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetWorkItemWorkLogs")]
    [Description("Retrieves the time tracking entries (work logs) recorded on a specific work item.")]
    public async Task<string> GetWorkLogsAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the work item.")]
        Guid workItemId,
        [Description("Max results. Default is 10.")]
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        Result<PaginatedResult<WorkLogDto>> result =
            await sender.Send(new GetWorkLogsQuery(projectId, workItemId, limit, null, []), cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(GetWorkLogsAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value.Items);
    }

    [KernelFunction("LogWorkOnWorkItem")]
    [Description("Records time worked on a specific work item.")]
    public async Task<string> LogWorkAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the work item.")]
        Guid workItemId,
        [Description("The number of hours worked (e.g., 1.5 for 1 hour 30 minutes).")]
        decimal dedication,
        [Description("The date the work was performed (ISO format: yyyy-MM-dd).")]
        string date,
        [Description("A short description of what was done during this session.")]
        string description = "",
        CancellationToken cancellationToken = default)
    {
        DateOnly parsedDate = DateOnly.ParseExact(
            date,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture
        );

        Result<WorkLogDto> result = await sender.Send(
            new LogWorkCommand(projectId, workItemId, dedication, parsedDate, description), cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(LogWorkAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("UpdateWorkLog")]
    [Description("Updates an existing work log entry. Only the fields you provide will be changed.")]
    public async Task<string> UpdateWorkLogAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the work item.")]
        Guid workItemId,
        [Description(
            "The unique identifier (GUID) of the work log to update. Find this ID first using 'GetWorkItemWorkLogs'.")]
        Guid logId,
        [Description("New hours worked. Leave null to keep current.")]
        decimal? dedication = null,
        [Description("New date (ISO format: yyyy-MM-dd). Leave null to keep current.")]
        string? date = null,
        [Description("New description. Leave null to keep current.")]
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        DateOnly? parsedDate = date is not null
            ? DateOnly.ParseExact(
                date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture
            )
            : null;

        Result<WorkLogDto> result = await sender.Send(
            new UpdateWorkLogCommand(projectId, workItemId, logId, dedication, parsedDate, description),
            cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(UpdateWorkLogAsync), result.Error.Description);
        }

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("DeleteWorkLog")]
    [Description("Deletes a time tracking entry from a work item.")]
    public async Task<string> DeleteWorkLogAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the work item.")]
        Guid workItemId,
        [Description(
            "The unique identifier (GUID) of the work log to delete. Find this ID first using 'GetWorkItemWorkLogs'.")]
        Guid logId,
        CancellationToken cancellationToken = default)
    {
        Result result =
            await sender.Send(new DeleteWorkLogCommand(projectId, workItemId, logId), cancellationToken);

        if (result.IsFailure)
        {
            return formatter.FormatFailure(nameof(DeleteWorkLogAsync), result.Error.Description);
        }

        return formatter.FormatMessage("Work log deleted successfully.");
    }
}
