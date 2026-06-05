using BuildingBlocks.Integration;

namespace Modules.WorkItems.Integration;

public sealed record WorkItemDeletedIntegrationEvent(
    Guid WorkItemId,
    Guid ProjectId) : IIntegrationEvent;
