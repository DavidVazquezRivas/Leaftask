using BuildingBlocks.Domain.Events;

namespace Modules.Projects.Domain.Events;

public sealed record FieldCreatedDomainEvent(
    Guid FieldId,
    string Name,
    bool IsOptional,
    Guid FieldTypeId,
    IReadOnlyList<Guid> WorkItemTypeIds) : IDomainEvent;
