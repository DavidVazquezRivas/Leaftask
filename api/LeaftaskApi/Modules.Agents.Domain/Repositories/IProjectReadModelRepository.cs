using Modules.Agents.Domain.Entities;

namespace Modules.Agents.Domain.Repositories;

public interface IProjectReadModelRepository
{
    Task<ProjectReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ProjectReadModel model, CancellationToken cancellationToken = default);
    void Remove(ProjectReadModel model);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
