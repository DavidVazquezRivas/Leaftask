using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Domain.Repositories;

public interface IUserReadModelRepository
{
    Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserReadModel userReadModel, CancellationToken cancellationToken = default);
}
