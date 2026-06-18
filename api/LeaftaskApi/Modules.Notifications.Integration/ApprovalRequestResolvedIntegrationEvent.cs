using BuildingBlocks.Integration;

namespace Modules.Notifications.Integration;

public sealed record ApprovalRequestResolvedIntegrationEvent(
    Guid RequestId,
    string ContextType,
    Guid ContextId,
    Guid TargetId,
    string Status,
    string? ActionType,
    string? ActionPayload) : IIntegrationEvent;
