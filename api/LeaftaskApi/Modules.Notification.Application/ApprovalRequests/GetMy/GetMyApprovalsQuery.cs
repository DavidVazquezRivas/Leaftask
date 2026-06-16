using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Notification.Application.ApprovalRequests.GetMy;

public sealed class GetMyApprovalsQuery : IPaginatedQuery<Result<PaginatedResult<ApprovalDto>>>
{
    public int Limit { get; init; } = 10;
    public string? Cursor { get; init; }
    public IReadOnlyCollection<string> Sort { get; init; } = [];
}
