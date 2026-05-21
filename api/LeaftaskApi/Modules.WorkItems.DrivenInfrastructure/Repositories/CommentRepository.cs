using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Repositories;

public sealed class CommentRepository(WorkItemsDbContext dbContext) : ICommentRepository
{
    public async Task AddAsync(WorkItemComment comment, CancellationToken cancellationToken = default) =>
        await dbContext.WorkItemComments.AddAsync(comment, cancellationToken);

    public async Task<WorkItemComment?> GetByIdTrackedAsync(
        Guid commentId,
        Guid workItemId,
        CancellationToken cancellationToken = default) =>
        await dbContext.WorkItemComments
            .Include(c => c.User)
            .Where(c => c.Id == commentId && EF.Property<Guid>(c, "work_item_id") == workItemId)
            .FirstOrDefaultAsync(cancellationToken);

    public void Remove(WorkItemComment comment) =>
        dbContext.WorkItemComments.Remove(comment);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
