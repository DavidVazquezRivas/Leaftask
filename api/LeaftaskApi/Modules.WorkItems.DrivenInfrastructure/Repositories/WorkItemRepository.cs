using Modules.WorkItems.Domain.Repositories;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Repositories;

public sealed class WorkItemRepository(WorkItemsDbContext dbContext) : IWorkItemRepository
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
