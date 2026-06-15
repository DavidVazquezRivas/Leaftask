using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Notification.Application.ApprovalRequests.GetMy;

public sealed class GetMyApprovalsQueryHandler(
    IGetMyApprovalsQueryService service,
    IUserContext userContext)
    : IQueryHandler<GetMyApprovalsQuery, Result<PaginatedResult<ApprovalDto>>>
{
    public async Task<Result<PaginatedResult<ApprovalDto>>> Handle(
        GetMyApprovalsQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResult<ApprovalDto> approvals = await service.GetMyApprovalsAsync(
            userContext.UserId,
            request.Limit,
            request.Cursor,
            cancellationToken);

        return Result.Success(approvals);
    }
}
