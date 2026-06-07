using Modules.Agents.Domain.Entities;

namespace Modules.Agents.Domain.Repositories;

public interface IAgentRepository
{
    Task<Agent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Agent?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Agent>> GetByEventTriggerAsync(string eventType, CancellationToken cancellationToken = default);
    Task AddAsync(Agent agent, CancellationToken cancellationToken = default);
    Task RemoveAsync(Agent agent, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
