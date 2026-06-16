using Modules.Agents.Domain.Entities.Execution;

namespace Modules.Agents.Domain.Repositories;

public interface IAgentExecutionMessageRepository
{
    Task<IReadOnlyList<AgentExecutionMessage>> GetByExecutionIdAsync(Guid executionId, CancellationToken cancellationToken = default);
    Task AddAsync(AgentExecutionMessage message, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<AgentExecutionMessage> messages, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
