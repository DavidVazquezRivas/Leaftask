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
    string TypeName,
    Guid StatusId,
    string StatusName,
    Guid? ParentId);

public sealed record WorkItemAssigneeDto(Guid Id, string FirstName, string LastName);
