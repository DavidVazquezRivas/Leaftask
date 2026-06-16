using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Notification.Application.ApprovalRequests.GetMy;

namespace Modules.Notification.Application.ApprovalRequests.AddComment;

public sealed record AddApprovalCommentCommand(Guid ApprovalId, string Content)
    : ICommand<Result<ApprovalCommentDto>>;
