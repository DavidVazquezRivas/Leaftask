using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.Configuration.GetWorkItemStatuses;

public sealed class GetWorkItemStatusesQueryHandler(IGetWorkItemStatusesQueryService queryService)
    : IQueryHandler<GetWorkItemStatusesQuery, Result<IReadOnlyList<WorkItemStatusDto>>>
{
    public async Task<Result<IReadOnlyList<WorkItemStatusDto>>> Handle(
        GetWorkItemStatusesQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<WorkItemStatusDto> statuses = await queryService.GetWorkItemStatusesAsync(cancellationToken);
        return Result.Success(statuses);
    }
}
