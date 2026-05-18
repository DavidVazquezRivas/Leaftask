using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Application.Configuration.GetWorkItemStatuses;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Queries;

public sealed class GetWorkItemStatusesQueryService(WorkItemsDbContext dbContext)
    : IGetWorkItemStatusesQueryService
{
    public async Task<IReadOnlyList<WorkItemStatusDto>> GetWorkItemStatusesAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.WorkItemStatuses
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Select(s => new WorkItemStatusDto(s.Id, s.Name))
            .ToListAsync(cancellationToken);
}
