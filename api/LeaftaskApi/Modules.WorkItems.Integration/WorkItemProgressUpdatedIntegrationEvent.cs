using BuildingBlocks.Integration;

namespace Modules.WorkItems.Integration;

public sealed record WorkItemProgressUpdatedIntegrationEvent(
    Guid WorkItemId,
    Guid ProjectId,
    float OldProgress,
    float NewProgress) : IIntegrationEvent;
