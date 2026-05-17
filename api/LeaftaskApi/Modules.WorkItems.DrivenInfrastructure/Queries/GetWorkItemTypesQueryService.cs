using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Application.Configuration.GetWorkItemTypes;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Queries;

public sealed class GetWorkItemTypesQueryService(WorkItemsDbContext dbContext)
    : IGetWorkItemTypesQueryService
{
    public async Task<IReadOnlyList<WorkItemTypeDto>> GetWorkItemTypesAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.WorkItemTypes
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new WorkItemTypeDto(t.Id, t.Name))
            .ToListAsync(cancellationToken);
}
