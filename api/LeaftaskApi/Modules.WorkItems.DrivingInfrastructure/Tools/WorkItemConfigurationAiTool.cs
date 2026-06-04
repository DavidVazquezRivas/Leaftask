using System.ComponentModel;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.WorkItems.Application.Configuration.GetWorkItemStatuses;
using Modules.WorkItems.Application.Configuration.GetWorkItemTypes;

namespace Modules.WorkItems.DrivingInfrastructure.Tools;

public class WorkItemConfigurationAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetWorkItemTypes")]
    [Description(
        "Retrieves all available work item types (e.g., Task, Bug, Story, Epic). Use this to resolve type names to their GUIDs before creating or filtering work items.")]
    public async Task<string> GetWorkItemTypesAsync(CancellationToken cancellationToken = default)
    {
        Result<IReadOnlyList<WorkItemTypeDto>> result =
            await sender.Send(new GetWorkItemTypesQuery(), cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(GetWorkItemTypesAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("GetWorkItemStatuses")]
    [Description(
        "Retrieves all available work item statuses (e.g., To Do, In Progress, Done). Use this to resolve status names to their GUIDs before creating or updating work items.")]
    public async Task<string> GetWorkItemStatusesAsync(CancellationToken cancellationToken = default)
    {
        Result<IReadOnlyList<WorkItemStatusDto>> result =
            await sender.Send(new GetWorkItemStatusesQuery(), cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(GetWorkItemStatusesAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value);
    }
}
