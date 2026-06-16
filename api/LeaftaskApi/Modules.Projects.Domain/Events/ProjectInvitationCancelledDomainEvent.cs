using BuildingBlocks.Domain.Events;

namespace Modules.Projects.Domain.Events;

public sealed record ProjectInvitationCancelledDomainEvent(
    Guid InvitationId,
    Guid ProjectId,
    Guid InviteeId) : IDomainEvent;
