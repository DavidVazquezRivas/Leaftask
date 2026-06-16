using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Notification.Application.Notifications.MarkAllAsRead;

public sealed record MarkAllNotificationsAsReadCommand : ICommand<Result>;
