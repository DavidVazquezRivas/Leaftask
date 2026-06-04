using Microsoft.EntityFrameworkCore;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.DrivenInfrastructure.Persistence;

namespace Modules.Agents.DrivenInfrastructure.Repositories;

public sealed class AgentRepository(AgentsDbContext dbContext) : IAgentRepository
{
    public async Task<Agent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Agents
            .Include(a => a.ModelConfig)
                .ThenInclude(c => c.Model)
                    .ThenInclude(m => m.Provider)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task AddAsync(Agent agent, CancellationToken cancellationToken = default) =>
        await dbContext.Agents.AddAsync(agent, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
