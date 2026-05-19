using BuildingBlocks.Integration;

namespace Modules.Projects.Integration;

public sealed record FieldUpdatedIntegrationEvent(
    Guid FieldId,
    string Name,
    bool IsOptional,
    Guid FieldTypeId,
    IReadOnlyList<Guid> WorkItemTypeIds) : IIntegrationEvent;
