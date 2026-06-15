using BuildingBlocks.Domain.Events;

namespace Modules.Organizations.Domain.Events;

public sealed record OrganizationMemberRoleChangedDomainEvent(
    Guid OrganizationId,
    Guid UserId,
    IReadOnlyCollection<OrganizationPermissionEntry> NewPermissions) : IDomainEvent;
