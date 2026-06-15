using BuildingBlocks.Application.Commands;
using Modules.Notification.Domain.Entities.Notification;

namespace Modules.Notification.Application.Notifications.Create;

public sealed record CreateNotificationCommand(
    NotificationType Type,
    Guid ContextId,
    Guid TargetId,
    Guid RecipientId,
    Guid? ActorId = null) : ICommand;
