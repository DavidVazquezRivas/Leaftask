using System.ComponentModel;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.WorkItems.Application.WorkItems;
using Modules.WorkItems.Application.WorkItems.Create;
using Modules.WorkItems.Application.WorkItems.Delete;
using Modules.WorkItems.Application.WorkItems.GetProjectWorkItems;
using Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;
using Modules.WorkItems.Application.WorkItems.Update;

namespace Modules.WorkItems.DrivingInfrastructure.Tools;

public class WorkItemManagementAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetProjectWorkItems")]
    [Description(
        "Retrieves a list of work items for a specific project. Useful to browse tasks, bugs, stories, and epics. If you only have the project name, resolve it first using 'GetOrganizationProjects' or 'GetMyProjects'.")]
    public async Task<string> GetProjectWorkItemsAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("Max results. Keep it low to avoid context overload. Default is 10.")]
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        GetProjectWorkItemsQuery query = new()
        {
            ProjectId = projectId,
            Limit = limit,
            Cursor = null,
            Sort = []
        };

        Result<PaginatedResult<WorkItemListDto>> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(GetProjectWorkItemsAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value.Items);
    }

    [KernelFunction("GetWorkItemDetails")]
    [Description(
        "Retrieves the full details of a work item, including its description, assignee, attachments, comments, activity log, and custom fields.")]
    public async Task<string> GetWorkItemDetailsAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description(
            "The unique identifier (GUID) of the work item. Find this ID first using 'GetProjectWorkItems'.")]
        Guid workItemId,
        CancellationToken cancellationToken = default)
    {
        Result<WorkItemDetailDto> result =
            await sender.Send(new GetWorkItemDetailsQuery(projectId, workItemId), cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(GetWorkItemDetailsAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("CreateWorkItem")]
    [Description("Creates a new work item (task, bug, story, or epic) within a project.")]
    public async Task<string> CreateWorkItemAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The title of the work item.")]
        string title,
        [Description(
            "The unique identifier (GUID) of the work item type. Use 'GetWorkItemTypes' to resolve type names.")]
        Guid typeId,
        [Description(
            "The unique identifier (GUID) of the initial status. Use 'GetWorkItemStatuses' to resolve status names.")]
        Guid statusId,
        [Description(
            "The unique identifier (GUID) of the parent work item (e.g., the Epic this task belongs to).")]
        Guid parentId,
        [Description("A detailed description of the work item.")]
        string description = "",
        [Description("The estimated effort in hours.")]
        decimal estimation = 0,
        [Description(
            "The unique identifier (GUID) of the user to assign this work item to. Leave null if unassigned.")]
        Guid? assigneeId = null,
        CancellationToken cancellationToken = default)
    {
        CreateWorkItemCommand command = new(
            projectId,
            title,
            description,
            estimation,
            typeId,
            statusId,
            assigneeId,
            parentId,
            new Dictionary<Guid, string>());

        Result<WorkItemDetailDto> result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(CreateWorkItemAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("UpdateWorkItem")]
    [Description(
        "Updates one or more properties of an existing work item. Only the fields you provide will be changed; leave others as null to keep current values.")]
    public async Task<string> UpdateWorkItemAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description(
            "The unique identifier (GUID) of the work item to update. Find this ID first using 'GetProjectWorkItems'.")]
        Guid workItemId,
        [Description("New title. Leave null to keep current.")]
        string? title = null,
        [Description("New description. Leave null to keep current.")]
        string? description = null,
        [Description(
            "New status GUID. Use 'GetWorkItemStatuses' to resolve status names. Leave null to keep current.")]
        Guid? statusId = null,
        [Description("New type GUID. Use 'GetWorkItemTypes' to resolve type names. Leave null to keep current.")]
        Guid? typeId = null,
        [Description("New assignee GUID. Only applies when updateAssignee is true.")]
        Guid? assigneeId = null,
        [Description("Set to true to update the assignee (pass null assigneeId to unassign).")]
        bool updateAssignee = false,
        [Description("New progress percentage (0–100). Leave null to keep current.")]
        int? progress = null,
        [Description("New estimated effort in hours. Leave null to keep current.")]
        decimal? estimation = null,
        [Description("New deadline (ISO 8601). Leave null to keep current.")]
        DateTime? limitDate = null,
        CancellationToken cancellationToken = default)
    {
        UpdateWorkItemCommand command = new(
            projectId,
            workItemId,
            title,
            description,
            statusId,
            typeId,
            assigneeId,
            updateAssignee,
            progress,
            estimation,
            limitDate,
            null,
            false,
            new Dictionary<Guid, string>());

        Result<WorkItemDetailDto> result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(UpdateWorkItemAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("DeleteWorkItem")]
    [Description(
        "Permanently deletes a work item from a project. Use carefully as this cannot be undone.")]
    public async Task<string> DeleteWorkItemAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description(
            "The unique identifier (GUID) of the work item to delete. Find this ID first using 'GetProjectWorkItems'.")]
        Guid workItemId,
        CancellationToken cancellationToken = default)
    {
        Result result = await sender.Send(new DeleteWorkItemCommand(projectId, workItemId), cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(DeleteWorkItemAsync), result.Error.Description);

        return formatter.FormatMessage("Work item deleted successfully.");
    }
}
