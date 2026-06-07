using Microsoft.EntityFrameworkCore;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.DrivenInfrastructure.Persistence;

namespace Modules.Agents.DrivenInfrastructure.Repositories;

public sealed class AgentExecutionRepository(AgentsDbContext dbContext) : IAgentExecutionRepository
{
    public async Task<List<AgentExecution>> GetPendingBatchAsync(int batchSize,
        CancellationToken cancellationToken = default) =>
        await dbContext.AgentExecutions
            .Include(e => e.Agent)
                .ThenInclude(a => a.ModelConfig)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(m => m.Provider)
            .Include(e => e.Agent)
                .ThenInclude(a => a.EventTriggers)
            .Where(e => e.Status == ExecutionStatus.Pending)
            .OrderBy(e => e.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

    public async Task<AgentExecution?> GetByIdTrackedAsync(Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.AgentExecutions
            .Include(e => e.Agent)
                .ThenInclude(a => a.ModelConfig)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(m => m.Provider)
            .Include(e => e.Agent)
                .ThenInclude(a => a.EventTriggers)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task AddAsync(AgentExecution entry, CancellationToken cancellationToken = default) =>
        await dbContext.AgentExecutions.AddAsync(entry, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
