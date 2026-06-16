using BuildingBlocks.Integration;

namespace Modules.Projects.Integration;

public sealed record ProjectInvitationCreatedIntegrationEvent(
    Guid InvitationId,
    Guid ProjectId,
    Guid InviteeId) : IIntegrationEvent;
