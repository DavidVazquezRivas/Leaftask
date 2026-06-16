using BuildingBlocks.Integration;

namespace Modules.Organizations.Integration;

public sealed record AffectedMemberPermissions(Guid UserId, IReadOnlyCollection<OrganizationPermissionEntry> Permissions);

public sealed record OrganizationRolePermissionsUpdatedIntegrationEvent(
    Guid OrganizationId,
    IReadOnlyCollection<AffectedMemberPermissions> AffectedMembers) : IIntegrationEvent;
