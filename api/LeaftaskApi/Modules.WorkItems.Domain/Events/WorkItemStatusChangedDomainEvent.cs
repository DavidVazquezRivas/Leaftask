using BuildingBlocks.Domain.Events;

namespace Modules.WorkItems.Domain.Events;

public sealed record WorkItemStatusChangedDomainEvent(
    Guid WorkItemId,
    Guid ProjectId,
    Guid OldStatusId,
    Guid NewStatusId) : IDomainEvent;
