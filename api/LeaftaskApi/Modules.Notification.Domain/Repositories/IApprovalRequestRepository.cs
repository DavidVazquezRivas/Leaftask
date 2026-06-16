using Modules.Notification.Domain.Entities.Approval;

namespace Modules.Notification.Domain.Repositories;

public interface IApprovalRequestRepository
{
    Task AddAsync(ApprovalRequest request, CancellationToken ct = default);
    Task<ApprovalRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
