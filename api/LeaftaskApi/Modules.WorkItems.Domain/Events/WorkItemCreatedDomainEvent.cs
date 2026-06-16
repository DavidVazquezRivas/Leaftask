using BuildingBlocks.Domain.Events;

namespace Modules.WorkItems.Domain.Events;

public sealed record WorkItemCreatedDomainEvent(
    Guid WorkItemId,
    Guid ProjectId,
    string Title,
    Guid StatusId,
    Guid TypeId,
    Guid? AssigneeId) : IDomainEvent;
