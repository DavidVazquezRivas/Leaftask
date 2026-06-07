using Modules.Agents.Domain.Entities.Execution;

namespace Modules.Agents.Domain.Repositories;

public interface IAgentExecutionPendingEventRepository
{
    Task<List<AgentExecutionPendingEvent>> GetUnresolvedAsync(string eventType, string correlationId, CancellationToken cancellationToken = default);
    Task<bool> HasUnresolvedAsync(Guid executionId, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<AgentExecutionPendingEvent> events, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
