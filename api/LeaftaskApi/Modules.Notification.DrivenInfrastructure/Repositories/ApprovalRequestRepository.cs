using Microsoft.EntityFrameworkCore;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.Domain.Repositories;
using Modules.Notification.DrivenInfrastructure.Persistence;

namespace Modules.Notification.DrivenInfrastructure.Repositories;

public sealed class ApprovalRequestRepository(NotificationsDbContext dbContext) : IApprovalRequestRepository
{
    public async Task AddAsync(ApprovalRequest request, CancellationToken ct = default) =>
        await dbContext.ApprovalRequests.AddAsync(request, ct);

    public async Task<ApprovalRequest?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await dbContext.ApprovalRequests
            .Include(a => a.Requester)
            .Include(a => a.ApproverRejecter)
            .Include(a => a.Comments).ThenInclude(c => c.CreatedBy)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await dbContext.SaveChangesAsync(ct);
}
