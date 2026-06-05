using BuildingBlocks.Integration;

namespace Modules.WorkItems.Integration;

public sealed record WorkItemAssigneeChangedIntegrationEvent(
    Guid WorkItemId,
    Guid ProjectId,
    Guid? OldAssigneeId,
    Guid? NewAssigneeId) : IIntegrationEvent;
