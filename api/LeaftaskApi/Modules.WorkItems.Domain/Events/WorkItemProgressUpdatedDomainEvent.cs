using BuildingBlocks.Domain.Events;

namespace Modules.WorkItems.Domain.Events;

public sealed record WorkItemProgressUpdatedDomainEvent(
    Guid WorkItemId,
    Guid ProjectId,
    float OldProgress,
    float NewProgress) : IDomainEvent;
