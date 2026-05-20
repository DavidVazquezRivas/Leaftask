using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Repositories;

public sealed class WorkLogRepository(WorkItemsDbContext dbContext) : IWorkLogRepository
{
    public async Task AddAsync(WorkLog workLog, CancellationToken cancellationToken = default) =>
        await dbContext.WorkLogs.AddAsync(workLog, cancellationToken);

    public async Task<WorkLog?> GetByIdTrackedAsync(
        Guid workLogId,
        Guid workItemId,
        CancellationToken cancellationToken = default) =>
        await dbContext.WorkLogs
            .Include(wl => wl.User)
            .Where(wl => wl.Id == workLogId && EF.Property<Guid>(wl, "work_item_id") == workItemId)
            .FirstOrDefaultAsync(cancellationToken);

    public void Remove(WorkLog workLog) =>
        dbContext.WorkLogs.Remove(workLog);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
