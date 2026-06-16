using BuildingBlocks.Integration;

namespace Modules.Organizations.Integration;

public sealed record OrganizationMemberJoinedIntegrationEvent(
    Guid OrganizationId,
    Guid UserId,
    IReadOnlyCollection<OrganizationPermissionEntry> Permissions) : IIntegrationEvent;
