using BuildingBlocks.Integration;

namespace Modules.Projects.Integration;

public sealed record ProjectInvitationCancelledIntegrationEvent(
    Guid InvitationId,
    Guid ProjectId,
    Guid InviteeId) : IIntegrationEvent;
