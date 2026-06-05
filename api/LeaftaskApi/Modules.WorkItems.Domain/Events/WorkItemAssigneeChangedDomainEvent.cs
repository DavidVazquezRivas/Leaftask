using BuildingBlocks.Domain.Events;

namespace Modules.WorkItems.Domain.Events;

public sealed record WorkItemAssigneeChangedDomainEvent(
    Guid WorkItemId,
    Guid ProjectId,
    Guid? OldAssigneeId,
    Guid? NewAssigneeId) : IDomainEvent;
