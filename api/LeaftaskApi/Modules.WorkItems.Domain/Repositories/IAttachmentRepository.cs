using Modules.WorkItems.Domain.Entities;

namespace Modules.WorkItems.Domain.Repositories;

public interface IAttachmentRepository
{
    Task AddAsync(Attachment attachment, CancellationToken cancellationToken = default);
    Task<Attachment?> GetByIdTrackedAsync(Guid attachmentId, Guid workItemId, CancellationToken cancellationToken = default);
    void Remove(Attachment attachment);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
