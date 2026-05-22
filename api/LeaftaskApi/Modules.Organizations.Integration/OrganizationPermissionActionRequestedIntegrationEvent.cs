using BuildingBlocks.Integration;

namespace Modules.Organizations.Integration;

public sealed record OrganizationPermissionActionRequestedIntegrationEvent(
    Guid OrganizationId,
    Guid RequestedByUserId,
    string PermissionName,
    string ActionName,
    string ActionPayload) : IIntegrationEvent;
