using Microsoft.EntityFrameworkCore;
using Modules.Notification.Domain.Entities;
using Modules.Notification.Domain.Repositories;
using Modules.Notification.DrivenInfrastructure.Persistence;

namespace Modules.Notification.DrivenInfrastructure.Repositories;

public sealed class UserReadModelRepository(NotificationsDbContext dbContext) : IUserReadModelRepository
{
    public async Task AddAsync(UserReadModel user, CancellationToken ct = default) =>
        await dbContext.UserReadModels.AddAsync(user, ct);

    public async Task<UserReadModel?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await dbContext.UserReadModels.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default) =>
        await dbContext.UserReadModels.AsNoTracking().AnyAsync(u => u.Id == id, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await dbContext.SaveChangesAsync(ct);
}
