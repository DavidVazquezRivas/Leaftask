namespace Modules.Projects.Application.Fields.GetProjectCustomFields;

public sealed record CustomFieldDto(
    Guid Id,
    string Name,
    Guid Type,
    IReadOnlyList<CustomFieldOptionDto> Options,
    bool Required,
    IReadOnlyList<CustomFieldWorkItemTypeDto> AppliesTo);

public sealed record CustomFieldWorkItemTypeDto(Guid Id, string Name);
