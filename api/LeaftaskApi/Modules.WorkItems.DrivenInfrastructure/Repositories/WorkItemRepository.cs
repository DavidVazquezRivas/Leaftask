using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Repositories;

public sealed class WorkItemRepository(WorkItemsDbContext dbContext) : IWorkItemRepository
{
    public async Task<int> GetNextCodeAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        int maxCode = await dbContext.WorkItems
            .Where(wi => EF.Property<Guid>(wi, "project_read_model_id") == projectId)
            .Select(wi => (int?)wi.Code)
            .MaxAsync(cancellationToken) ?? 0;

        return maxCode + 1;
    }

    public async Task<bool> ExistsInProjectAsync(
        Guid workItemId,
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await dbContext.WorkItems
            .AnyAsync(wi => wi.Id == workItemId
                            && EF.Property<Guid>(wi, "project_read_model_id") == projectId,
                cancellationToken);

    public async Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default) =>
        await dbContext.WorkItems.AddAsync(workItem, cancellationToken);

    public async Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.WorkItems
            .Include(wi => wi.Project)
            .Include(wi => wi.Status)
            .Include(wi => wi.Type)
            .Include(wi => wi.Asignee)
            .AsNoTracking()
            .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);

    public async Task<WorkItem?> GetByIdTrackedAsync(
        Guid id,
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await dbContext.WorkItems
            .Include(wi => wi.Project)
            .Include(wi => wi.Status)
            .Include(wi => wi.Type)
            .Include(wi => wi.Asignee)
            .Where(wi => wi.Id == id && EF.Property<Guid>(wi, "project_read_model_id") == projectId)
            .FirstOrDefaultAsync(cancellationToken);

    public void Remove(WorkItem workItem) =>
        dbContext.WorkItems.Remove(workItem);

    public async Task AddActivityLogAsync(ActivityLog activityLog, CancellationToken cancellationToken = default) =>
        await dbContext.ActivityLogs.AddAsync(activityLog, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
