using Microsoft.EntityFrameworkCore;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.DrivenInfrastructure.Persistence;

namespace Modules.Agents.DrivenInfrastructure.Repositories;

public sealed class AgentExecutionPendingEventRepository(AgentsDbContext dbContext)
    : IAgentExecutionPendingEventRepository
{
    public async Task<List<AgentExecutionPendingEvent>> GetUnresolvedAsync(string eventType, string correlationId,
        CancellationToken cancellationToken = default) =>
        await dbContext.AgentExecutionPendingEvents
            .Where(pe => pe.EventType == eventType
                         && pe.CorrelationId == correlationId
                         && !pe.IsResolved)
            .ToListAsync(cancellationToken);

    public async Task<bool> HasUnresolvedAsync(Guid executionId, CancellationToken cancellationToken = default) =>
        await dbContext.AgentExecutionPendingEvents
            .AnyAsync(pe => pe.ExecutionId == executionId && !pe.IsResolved, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<AgentExecutionPendingEvent> events,
        CancellationToken cancellationToken = default) =>
        await dbContext.AgentExecutionPendingEvents.AddRangeAsync(events, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
