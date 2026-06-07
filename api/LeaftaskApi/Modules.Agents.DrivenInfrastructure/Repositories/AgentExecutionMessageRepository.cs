using Microsoft.EntityFrameworkCore;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.DrivenInfrastructure.Persistence;

namespace Modules.Agents.DrivenInfrastructure.Repositories;

public sealed class AgentExecutionMessageRepository(AgentsDbContext dbContext) : IAgentExecutionMessageRepository
{
    public async Task<IReadOnlyList<AgentExecutionMessage>> GetByExecutionIdAsync(Guid executionId,
        CancellationToken cancellationToken = default) =>
        await dbContext.AgentExecutionMessages
            .AsNoTracking()
            .Where(m => m.ExecutionId == executionId)
            .OrderBy(m => m.SequenceNumber)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(AgentExecutionMessage message, CancellationToken cancellationToken = default) =>
        await dbContext.AgentExecutionMessages.AddAsync(message, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<AgentExecutionMessage> messages,
        CancellationToken cancellationToken = default) =>
        await dbContext.AgentExecutionMessages.AddRangeAsync(messages, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
