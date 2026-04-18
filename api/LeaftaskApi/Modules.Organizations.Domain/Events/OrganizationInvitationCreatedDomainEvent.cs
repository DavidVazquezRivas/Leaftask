using BuildingBlocks.Domain.Events;

namespace Modules.Organizations.Domain.Events;

public sealed record OrganizationInvitationCreatedDomainEvent(
    Guid OrganizationInvitationId,
    Guid OrganizationId,
    Guid UserId,
    Guid OrganizationRoleId) : IDomainEvent;
