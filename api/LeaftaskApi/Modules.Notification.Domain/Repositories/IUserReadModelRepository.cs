using Modules.Notification.Domain.Entities;

namespace Modules.Notification.Domain.Repositories;

public interface IUserReadModelRepository
{
    Task AddAsync(UserReadModel user, CancellationToken ct = default);
    Task<UserReadModel?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
