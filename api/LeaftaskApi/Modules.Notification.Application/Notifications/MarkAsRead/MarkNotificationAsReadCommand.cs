using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Notification.Application.Notifications.MarkAsRead;

public sealed record MarkNotificationAsReadCommand(Guid NotificationId) : ICommand<Result>;
