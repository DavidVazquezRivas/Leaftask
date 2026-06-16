using BuildingBlocks.Integration;

namespace Modules.Projects.Integration;

public sealed record ProjectPermissionActionRequestedIntegrationEvent(
    Guid ProjectId,
    Guid RequestedByUserId,
    string PermissionName,
    string ActionName,
    string ActionPayload) : IIntegrationEvent;
