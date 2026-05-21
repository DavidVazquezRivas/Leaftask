using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Repositories;

public sealed class AttachmentRepository(WorkItemsDbContext dbContext) : IAttachmentRepository
{
    public async Task AddAsync(Attachment attachment, CancellationToken cancellationToken = default) =>
        await dbContext.Attachments.AddAsync(attachment, cancellationToken);

    public async Task<Attachment?> GetByIdTrackedAsync(
        Guid attachmentId,
        Guid workItemId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Attachments
            .Include(a => a.User)
            .Where(a => a.Id == attachmentId && EF.Property<Guid>(a, "work_item_id") == workItemId)
            .FirstOrDefaultAsync(cancellationToken);

    public void Remove(Attachment attachment) =>
        dbContext.Attachments.Remove(attachment);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
