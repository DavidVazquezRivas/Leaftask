using Modules.Notification.Application.Notifications.GetMy;

namespace Modules.Notification.Application.ApprovalRequests.GetMy;

public sealed record ApprovalCommentDto(
    Guid Id,
    string Content,
    DateTime Timestamp,
    SimpleReferenceDto Author);

public sealed record ApprovalDto(
    Guid Id,
    string Status,
    SimpleReferenceDto Context,
    SimpleReferenceDto Target,
    SimpleReferenceDto Requester,
    DateTime CreatedAt,
    IReadOnlyCollection<ApprovalCommentDto> Comments);
