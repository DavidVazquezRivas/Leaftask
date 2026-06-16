using BuildingBlocks.Integration;

namespace Modules.Organizations.Integration;

public sealed record OrganizationMemberRoleChangedIntegrationEvent(
    Guid OrganizationId,
    Guid UserId,
    IReadOnlyCollection<OrganizationPermissionEntry> NewPermissions) : IIntegrationEvent;
