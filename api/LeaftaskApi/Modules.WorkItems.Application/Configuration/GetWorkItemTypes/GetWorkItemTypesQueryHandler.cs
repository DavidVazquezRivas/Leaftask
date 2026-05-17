using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.Configuration.GetWorkItemTypes;

public sealed class GetWorkItemTypesQueryHandler(IGetWorkItemTypesQueryService queryService)
    : IQueryHandler<GetWorkItemTypesQuery, Result<IReadOnlyList<WorkItemTypeDto>>>
{
    public async Task<Result<IReadOnlyList<WorkItemTypeDto>>> Handle(
        GetWorkItemTypesQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<WorkItemTypeDto> types = await queryService.GetWorkItemTypesAsync(cancellationToken);
        return Result.Success(types);
    }
}
