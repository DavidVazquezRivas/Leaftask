using Modules.Agents.Domain.Entities.Queue;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.DrivenInfrastructure.Persistence;

namespace Modules.Agents.DrivenInfrastructure.Repositories;

public sealed class AgentExecutionQueueRepository(AgentsDbContext dbContext) : IAgentExecutionQueueRepository
{
    public async Task AddAsync(AgentExecutionQueue entry, CancellationToken cancellationToken = default) =>
        await dbContext.AgentExecutionQueues.AddAsync(entry, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
