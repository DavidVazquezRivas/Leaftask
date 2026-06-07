using Modules.Projects.Domain.Entities.Member;

namespace Modules.Projects.Domain.Repositories;

public interface IAgentReadModelRepository
{
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AgentReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(AgentReadModel model, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
