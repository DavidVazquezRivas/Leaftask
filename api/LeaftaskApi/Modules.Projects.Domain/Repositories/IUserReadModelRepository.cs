using Modules.Projects.Domain.Entities.Owner;

namespace Modules.Projects.Domain.Repositories;

public interface IUserReadModelRepository
{
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(UserReadModel userReadModel, CancellationToken cancellationToken = default);
}
