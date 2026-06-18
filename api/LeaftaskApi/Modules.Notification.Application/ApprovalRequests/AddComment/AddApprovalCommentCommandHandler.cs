using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Notification.Application.ApprovalRequests.GetMy;
using Modules.Notification.Application.Notifications.GetMy;
using Modules.Notification.Domain.Entities;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.Domain.Errors;
using Modules.Notification.Domain.Repositories;

namespace Modules.Notification.Application.ApprovalRequests.AddComment;

public sealed class AddApprovalCommentCommandHandler(
    IApprovalRequestRepository approvalRequestRepository,
    IOrganizationPermissionReadModelRepository orgPermissionRepository,
    IProjectPermissionReadModelRepository projectPermissionRepository,
    IUserReadModelRepository userReadModelRepository,
    IUserContext userContext)
    : ICommandHandler<AddApprovalCommentCommand, Result<ApprovalCommentDto>>
{
    public async Task<Result<ApprovalCommentDto>> Handle(
        AddApprovalCommentCommand request,
        CancellationToken cancellationToken)
    {
        ApprovalRequest? approvalRequest =
            await approvalRequestRepository.GetByIdAsync(request.ApprovalId, cancellationToken);

        if (approvalRequest is null)
            return Result.Failure<ApprovalCommentDto>(ApprovalRequestErrors.NotFound);

        bool isRequester = approvalRequest.Requester.Id == userContext.UserId;
        if (!isRequester)
        {
            bool hasPermission = approvalRequest.ContextType switch
            {
                ContextType.Project => await projectPermissionRepository.ExistsAsync(
                    userContext.UserId, approvalRequest.ContextId, approvalRequest.PermissionName, 2, cancellationToken),
                _ => await orgPermissionRepository.ExistsAsync(
                    userContext.UserId, approvalRequest.ContextId, approvalRequest.PermissionName, 2, cancellationToken)
            };

            if (!hasPermission)
                return Result.Failure<ApprovalCommentDto>(ApprovalRequestErrors.Forbidden);
        }

        UserReadModel? author = await userReadModelRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (author is null)
            return Result.Failure<ApprovalCommentDto>(ApprovalRequestErrors.RequesterNotFound);

        RequestComment comment = approvalRequest.AddComment(author, request.Content);

        await approvalRequestRepository.AddCommentAsync(comment, cancellationToken);
        await approvalRequestRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(new ApprovalCommentDto(
            comment.Id,
            comment.Content,
            comment.CreatedAt,
            new SimpleReferenceDto(author.Id, $"{author.FirstName} {author.LastName}")));
    }
}
