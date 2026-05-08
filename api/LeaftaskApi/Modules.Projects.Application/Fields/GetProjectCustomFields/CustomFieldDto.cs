namespace Modules.Projects.Application.Fields.GetProjectCustomFields;

public sealed record CustomFieldDto(
    Guid Id,
    string Name,
    Guid Type,
    IReadOnlyList<CustomFieldOptionDto> Options,
    bool Required,
    IReadOnlyList<Guid> AppliesTo);
