using Modules.WorkItems.Domain.Entities;

namespace Modules.WorkItems.Domain.Repositories;

public interface IProjectReadModelRepository
{
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProjectReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ProjectReadModel projectReadModel, CancellationToken cancellationToken = default);
    void Remove(ProjectReadModel projectReadModel);
}
