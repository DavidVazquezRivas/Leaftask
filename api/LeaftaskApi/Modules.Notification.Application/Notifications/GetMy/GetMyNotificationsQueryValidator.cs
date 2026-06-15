using FluentValidation;

namespace Modules.Notification.Application.Notifications.GetMy;

public sealed class GetMyNotificationsQueryValidator : AbstractValidator<GetMyNotificationsQuery>
{
    private static readonly string[] ValidStatuses = ["all", "read", "unread"];

    public GetMyNotificationsQueryValidator()
    {
        RuleFor(q => q.Limit).InclusiveBetween(1, 100);
        RuleFor(q => q.Status).Must(s => ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status must be 'all', 'read', or 'unread'.");
    }
}
