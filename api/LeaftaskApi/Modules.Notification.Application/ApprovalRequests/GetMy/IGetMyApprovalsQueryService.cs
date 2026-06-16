using BuildingBlocks.Application.Queries;

namespace Modules.Notification.Application.ApprovalRequests.GetMy;

public interface IGetMyApprovalsQueryService
{
    Task<PaginatedResult<ApprovalDto>> GetMyApprovalsAsync(
        Guid userId,
        int limit,
        string? cursor,
        CancellationToken cancellationToken = default);
}
