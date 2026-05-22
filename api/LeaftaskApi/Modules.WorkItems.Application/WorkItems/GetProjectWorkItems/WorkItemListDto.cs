namespace Modules.WorkItems.Application.WorkItems.GetProjectWorkItems;

public sealed record WorkItemListDto(
    Guid Id,
    string Code,
    string Title,
    decimal? Estimation,
    float Progress,
    WorkItemAssigneeDto? Assignee,
    float? Dedication,
    Guid TypeId,
    Guid StatusId,
    Guid? ParentId);

public sealed record WorkItemAssigneeDto(Guid Id, string FirstName, string LastName);
