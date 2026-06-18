using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using Modules.Notification.Domain.Events;
using Modules.Notifications.Integration;

namespace Modules.Notification.Application.Events;

public sealed class NotificationsModuleEventMapper : IIntegrationEventMapper
{
    public IIntegrationEvent? Map(IDomainEvent domainEvent) =>
        domainEvent switch
        {
            ApprovalRequestResolvedDomainEvent e => new ApprovalRequestResolvedIntegrationEvent(
                e.RequestId,
                e.ContextType.ToString(),
                e.ContextId,
                e.TargetId,
                e.Status.ToString(),
                e.ActionType,
                e.ActionPayload),

            _ => null
        };
}
