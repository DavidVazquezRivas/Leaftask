using Modules.Agents.Domain.Entities.Model;

namespace Modules.Agents.Domain.Repositories;

public interface IModelRepository
{
    Task<IReadOnlyList<Model>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Model?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
