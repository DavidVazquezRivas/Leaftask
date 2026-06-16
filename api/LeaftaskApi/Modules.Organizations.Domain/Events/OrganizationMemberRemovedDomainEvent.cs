using BuildingBlocks.Domain.Events;

namespace Modules.Organizations.Domain.Events;

public sealed record OrganizationMemberRemovedDomainEvent(
    Guid OrganizationId,
    Guid UserId) : IDomainEvent;
