using Modules.Agents.Domain.Entities.Queue;

namespace Modules.Agents.Domain.Repositories;

public interface IAgentExecutionQueueRepository
{
    Task AddAsync(AgentExecutionQueue entry, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
