using BuildingBlocks.Domain.Result;

namespace Modules.Notification.Domain.Errors;

public static class NotificationErrors
{
    public static readonly Error NotFound = new("Notification.NotFound", "Notification not found", 404);
    public static readonly Error AlreadyRead = new("Notification.AlreadyRead", "Notification already read", 409);
    public static readonly Error Forbidden = new("Notification.Forbidden", "Access denied", 403);
}
