namespace Modules.Notification.Application.Notifications.GetMy;

public sealed record SimpleReferenceDto(Guid Id, string Name);

public sealed record NotificationDto(
    Guid Id,
    string Type,
    SimpleReferenceDto Context,
    SimpleReferenceDto Target,
    DateTime Timestamp,
    SimpleReferenceDto? Actor,
    bool Read);
