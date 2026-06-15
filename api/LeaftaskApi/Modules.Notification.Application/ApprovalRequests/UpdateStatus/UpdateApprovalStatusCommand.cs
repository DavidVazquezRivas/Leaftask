using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Notification.Application.ApprovalRequests.GetMy;

namespace Modules.Notification.Application.ApprovalRequests.UpdateStatus;

public sealed record UpdateApprovalStatusCommand(Guid ApprovalId, string Status)
    : ICommand<Result<ApprovalDto>>;
