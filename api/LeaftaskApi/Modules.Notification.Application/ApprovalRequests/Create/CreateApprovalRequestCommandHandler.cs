using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Notification.Domain.Entities;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.Domain.Errors;
using Modules.Notification.Domain.Repositories;

namespace Modules.Notification.Application.ApprovalRequests.Create;

public sealed class CreateApprovalRequestCommandHandler(
    IApprovalRequestRepository approvalRequestRepository,
    IUserReadModelRepository userReadModelRepository)
    : ICommandHandler<CreateApprovalRequestCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateApprovalRequestCommand request, CancellationToken cancellationToken)
    {
        UserReadModel? requester = await userReadModelRepository.GetByIdAsync(request.RequesterId, cancellationToken);
        if (requester is null)
            return Result.Failure<Guid>(ApprovalRequestErrors.RequesterNotFound);

        ApprovalRequest approvalRequest = ApprovalRequest.Create(request.ContextType, request.ContextId,
            request.RequesterId, request.PermissionName, requester, request.ActionType, request.ActionPayload);

        await approvalRequestRepository.AddAsync(approvalRequest, cancellationToken);
        await approvalRequestRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(approvalRequest.Id);
    }
}
