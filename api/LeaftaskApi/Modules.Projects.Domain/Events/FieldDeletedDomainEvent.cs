using BuildingBlocks.Domain.Events;

namespace Modules.Projects.Domain.Events;

public sealed record FieldDeletedDomainEvent(Guid FieldId) : IDomainEvent;
