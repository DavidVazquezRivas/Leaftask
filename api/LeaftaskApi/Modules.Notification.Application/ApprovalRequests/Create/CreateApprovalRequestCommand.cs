using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Notification.Domain.Entities.Approval;

namespace Modules.Notification.Application.ApprovalRequests.Create;

public sealed record CreateApprovalRequestCommand(
    ContextType ContextType,
    Guid ContextId,
    Guid RequesterId,
    string PermissionName) : ICommand<Result<Guid>>;
