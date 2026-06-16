using Modules.Agents.Domain.Entities.Execution;

namespace Modules.Agents.Domain.Repositories;

public interface IAgentExecutionRepository
{
    Task<List<AgentExecution>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken = default);
    Task<AgentExecution?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasActiveRegularExecutionAsync(Guid agentId, CancellationToken cancellationToken = default);
    Task<AgentExecution?> GetActiveRegularForAgentAsync(Guid agentId, CancellationToken cancellationToken = default);
    Task<List<AgentExecution>> GetTimedOutSuspendedAsync(TimeSpan threshold, CancellationToken cancellationToken = default);
    Task AddAsync(AgentExecution entry, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
