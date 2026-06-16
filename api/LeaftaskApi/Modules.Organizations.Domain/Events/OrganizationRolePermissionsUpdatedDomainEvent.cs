using BuildingBlocks.Domain.Events;

namespace Modules.Organizations.Domain.Events;

public sealed record AffectedMemberPermissions(Guid UserId, IReadOnlyCollection<OrganizationPermissionEntry> Permissions);

public sealed record OrganizationRolePermissionsUpdatedDomainEvent(
    Guid OrganizationId,
    IReadOnlyCollection<AffectedMemberPermissions> AffectedMembers) : IDomainEvent;
