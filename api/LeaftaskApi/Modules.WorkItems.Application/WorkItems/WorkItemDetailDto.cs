namespace Modules.WorkItems.Application.WorkItems;

public sealed record WorkItemDetailDto(
    Guid Id,
    string Code,
    string Title,
    string? Description,
    DateTime? LimitDate,
    WorkItemAssigneeDetailDto? Assignee,
    decimal? Estimation,
    WorkItemDedicationDto Dedication,
    float Progress,
    Guid TypeId,
    Guid StatusId,
    Guid? ParentId,
    IReadOnlyList<WorkItemAttachmentDto> Attachments,
    IReadOnlyList<WorkItemCommentDetailDto> Comments,
    IReadOnlyList<WorkItemLogEntryDto> Log,
    IReadOnlyList<WorkItemCustomFieldDto> CustomFields);

public sealed record WorkItemAssigneeDetailDto(Guid Id, string FullName);

public sealed record WorkItemDedicationDto(float Total, int Registers);

public sealed record WorkItemAttachmentDto(Guid Id, string FileName, Uri Url);

public sealed record WorkItemCommentDetailDto(
    Guid Id,
    WorkItemAssigneeDetailDto Author,
    string Content,
    DateTime CreatedAt,
    IReadOnlyList<WorkItemAttachmentDto> Attachments);

public sealed record WorkItemLogEntryDto(
    DateTime Timestamp,
    WorkItemAssigneeDetailDto User,
    string FieldName,
    WorkItemLogValueDto OldValue,
    WorkItemLogValueDto NewValue);

public sealed record WorkItemLogValueDto(string Value);

public sealed record WorkItemCustomFieldDto(Guid FieldId, string Name, string Value, Guid TypeId);
