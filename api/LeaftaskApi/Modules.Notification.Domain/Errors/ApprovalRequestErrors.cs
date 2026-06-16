using BuildingBlocks.Domain.Result;

namespace Modules.Notification.Domain.Errors;

public static class ApprovalRequestErrors
{
    public static readonly Error NotFound = new("ApprovalRequest.NotFound", "Approval request not found", 404);
    public static readonly Error AlreadyResolved = new("ApprovalRequest.AlreadyResolved", "Already approved or rejected", 409);
    public static readonly Error RequesterNotFound = new("ApprovalRequest.RequesterNotFound", "Requester user not found", 404);
    public static readonly Error Forbidden = new("ApprovalRequest.Forbidden", "You do not have permission to resolve this request", 403);
}
