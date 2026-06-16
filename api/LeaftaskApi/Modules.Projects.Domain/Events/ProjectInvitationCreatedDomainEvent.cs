using BuildingBlocks.Domain.Events;

namespace Modules.Projects.Domain.Events;

public sealed record ProjectInvitationCreatedDomainEvent(
    Guid InvitationId,
    Guid ProjectId,
    Guid InviteeId) : IDomainEvent;
