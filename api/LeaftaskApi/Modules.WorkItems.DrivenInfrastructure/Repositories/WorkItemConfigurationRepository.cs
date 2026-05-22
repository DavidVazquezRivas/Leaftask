using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Repositories;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Repositories;

public sealed class WorkItemConfigurationRepository(WorkItemsDbContext dbContext)
    : IWorkItemConfigurationRepository
{
    public async Task<WorkItemStatus?> GetStatusByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.WorkItemStatuses
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<WorkItemType?> GetTypeByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.WorkItemTypes
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<List<WorkItemType>> GetTypesByIdsAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken = default) =>
        await dbContext.WorkItemTypes
            .Where(t => ids.Contains(t.Id))
            .ToListAsync(cancellationToken);
}
