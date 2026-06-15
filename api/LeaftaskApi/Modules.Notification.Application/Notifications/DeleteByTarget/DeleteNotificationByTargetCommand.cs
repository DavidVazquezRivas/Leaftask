using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Notification.Application.Notifications.DeleteByTarget;

public sealed record DeleteNotificationByTargetCommand(Guid TargetId) : ICommand<Result>;
