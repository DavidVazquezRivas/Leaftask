using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Notification.Application.ApprovalRequests.GetMy;
using Modules.Notification.Application.Notifications.GetMy;
using Modules.Notification.Domain.Entities;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.Domain.Errors;
using Modules.Notification.Domain.Repositories;

namespace Modules.Notification.Application.ApprovalRequests.UpdateStatus;

public sealed class UpdateApprovalStatusCommandHandler(
    IApprovalRequestRepository approvalRequestRepository,
    IOrganizationPermissionReadModelRepository permissionReadModelRepository,
    IUserReadModelRepository userReadModelRepository,
    IUserContext userContext)
    : ICommandHandler<UpdateApprovalStatusCommand, Result<ApprovalDto>>
{
    public async Task<Result<ApprovalDto>> Handle(
        UpdateApprovalStatusCommand request,
        CancellationToken cancellationToken)
    {
        ApprovalRequest? approvalRequest =
            await approvalRequestRepository.GetByIdAsync(request.ApprovalId, cancellationToken);

        if (approvalRequest is null)
            return Result.Failure<ApprovalDto>(ApprovalRequestErrors.NotFound);

        if (approvalRequest.Status != RequestStatus.Pending)
            return Result.Failure<ApprovalDto>(ApprovalRequestErrors.AlreadyResolved);

        bool hasPermission = await permissionReadModelRepository.ExistsAsync(
            userContext.UserId, approvalRequest.ContextId, approvalRequest.PermissionName, 2, cancellationToken);

        if (!hasPermission)
            return Result.Failure<ApprovalDto>(ApprovalRequestErrors.Forbidden);

        UserReadModel? resolver = await userReadModelRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (resolver is null)
            return Result.Failure<ApprovalDto>(ApprovalRequestErrors.RequesterNotFound);

        if (request.Status.Equals("approved", StringComparison.OrdinalIgnoreCase))
            approvalRequest.Approve(resolver);
        else
            approvalRequest.Reject(resolver);

        await approvalRequestRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToDto(approvalRequest));
    }

    private static ApprovalDto MapToDto(ApprovalRequest ar) =>
        new(
            ar.Id,
            ar.Status switch
            {
                RequestStatus.Pending => "pending",
                RequestStatus.Approved => "approved",
                RequestStatus.Rejected => "rejected",
                _ => ar.Status.ToString()
            },
            new SimpleReferenceDto(ar.ContextId, "Organization"),
            new SimpleReferenceDto(ar.TargetId, "Permission Request"),
            new SimpleReferenceDto(ar.Requester.Id, $"{ar.Requester.FirstName} {ar.Requester.LastName}"),
            ar.CreatedAt,
            ar.Comments.Select(c => new ApprovalCommentDto(
                c.Id,
                c.Content,
                c.CreatedAt,
                new SimpleReferenceDto(c.CreatedBy.Id, $"{c.CreatedBy.FirstName} {c.CreatedBy.LastName}"))).ToArray());
}
