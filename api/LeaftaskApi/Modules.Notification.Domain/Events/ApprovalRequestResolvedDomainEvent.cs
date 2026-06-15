using BuildingBlocks.Domain.Events;
using Modules.Notification.Domain.Entities.Approval;

namespace Modules.Notification.Domain.Events;

public sealed record ApprovalRequestResolvedDomainEvent(
    Guid RequestId,
    ContextType ContextType,
    Guid ContextId,
    Guid TargetId,
    RequestStatus Status) : IDomainEvent;
