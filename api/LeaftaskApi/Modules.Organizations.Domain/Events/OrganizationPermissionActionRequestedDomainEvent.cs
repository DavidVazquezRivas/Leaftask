using BuildingBlocks.Domain.Events;

namespace Modules.Organizations.Domain.Events;

public sealed record OrganizationPermissionActionRequestedDomainEvent(
    Guid OrganizationId,
    Guid RequestedByUserId,
    string PermissionName,
    string ActionName) : IDomainEvent;
