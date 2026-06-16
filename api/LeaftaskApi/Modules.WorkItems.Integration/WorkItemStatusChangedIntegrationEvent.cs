using BuildingBlocks.Integration;

namespace Modules.WorkItems.Integration;

public sealed record WorkItemStatusChangedIntegrationEvent(
    Guid WorkItemId,
    Guid ProjectId,
    Guid OldStatusId,
    Guid NewStatusId) : IIntegrationEvent;
