using BuildingBlocks.Integration;

namespace Modules.WorkItems.Integration;

public sealed record WorkItemCreatedIntegrationEvent(
    Guid WorkItemId,
    Guid ProjectId,
    string Title,
    Guid StatusId,
    Guid TypeId,
    Guid? AssigneeId) : IIntegrationEvent;
