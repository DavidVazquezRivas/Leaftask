using BuildingBlocks.Domain.Events;

namespace Modules.Organizations.Domain.Events;

public sealed record OrganizationCreatedDomainEvent(
    Guid OrganizationId,
    Guid CreatorUserId) : IDomainEvent;
