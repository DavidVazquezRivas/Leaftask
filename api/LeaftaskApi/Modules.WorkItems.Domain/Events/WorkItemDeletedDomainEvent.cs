using BuildingBlocks.Domain.Events;

namespace Modules.WorkItems.Domain.Events;

public sealed record WorkItemDeletedDomainEvent(
    Guid WorkItemId,
    Guid ProjectId) : IDomainEvent;
