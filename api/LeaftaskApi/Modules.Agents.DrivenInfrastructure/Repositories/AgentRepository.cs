using Microsoft.EntityFrameworkCore;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.DrivenInfrastructure.Persistence;

namespace Modules.Agents.DrivenInfrastructure.Repositories;

public sealed class AgentRepository(AgentsDbContext dbContext) : IAgentRepository
{
    public async Task<Agent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await BuildAgentQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<Agent?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        await BuildAgentQuery()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Agent>> GetByEventTriggerAsync(
        string eventType,
        CancellationToken cancellationToken = default) =>
        await BuildAgentQuery()
            .AsNoTracking()
            .Where(a => a.EventTriggers.Any(t => t.Event == eventType))
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Agent agent, CancellationToken cancellationToken = default) =>
        await dbContext.Agents.AddAsync(agent, cancellationToken);

    public async Task RemoveAsync(Agent agent, CancellationToken cancellationToken = default)
    {
        await dbContext.AgentExecutions
            .Where(e => EF.Property<Guid>(e, "agent_id") == agent.Id)
            .ExecuteDeleteAsync(cancellationToken);

        dbContext.Agents.Remove(agent);
        dbContext.ModelConfigs.Remove(agent.ModelConfig);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);

    private IQueryable<Agent> BuildAgentQuery() =>
        dbContext.Agents
            .Include(a => a.ModelConfig)
                .ThenInclude(c => c.Model)
                    .ThenInclude(m => m.Provider)
            .Include(a => a.EventTriggers)
            .Include(a => a.TimeTriggers);
}
