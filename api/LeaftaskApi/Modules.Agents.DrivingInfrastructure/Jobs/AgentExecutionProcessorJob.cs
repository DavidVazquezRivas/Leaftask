using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Agents.Application.Services;
using Modules.Agents.Domain.Entities.Queue;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Quartz;

namespace Modules.Agents.DrivingInfrastructure.Jobs;

public sealed class AgentExecutionProcessorJob(
    IServiceScopeFactory scopeFactory,
    ILogger<AgentExecutionProcessorJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        AgentsDbContext dbContext = scope.ServiceProvider.GetRequiredService<AgentsDbContext>();
        AgentOrchestrator orchestrator = scope.ServiceProvider.GetRequiredService<AgentOrchestrator>();

        int batchSize = context.MergedJobDataMap.GetIntValue("BatchSize");
        if (batchSize == 0) batchSize = 5;

        List<AgentExecutionQueue> pending = await dbContext.AgentExecutionQueues
            .Include(q => q.Agent)
                .ThenInclude(a => a.ModelConfig)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(m => m.Provider)
            .Include(q => q.Agent)
                .ThenInclude(a => a.EventTriggers)
            .Where(q => q.Status == QueueStatus.Pending)
            .OrderBy(q => q.CreatedAt)
            .Take(batchSize)
            .ToListAsync(context.CancellationToken);

        if (pending.Count == 0)
            return;

        foreach (AgentExecutionQueue entry in pending)
        {
            QueueStatus newStatus;
            try
            {
                string userPrompt = entry.Agent.EventTriggers
                    .FirstOrDefault(t => entry.Payload.Contains(t.Event, StringComparison.Ordinal))
                    ?.UserPrompt ?? "Execute your scheduled task based on the provided context.";

                await orchestrator.ExecuteTaskAsync(
                    entry.Agent,
                    $"{userPrompt}\n\nContext: {entry.Payload}",
                    context.CancellationToken);

                newStatus = QueueStatus.Completed;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing agent {AgentId} from queue entry {EntryId}",
                    entry.Agent.Id, entry.Id);
                newStatus = QueueStatus.Failed;
            }

            await dbContext.AgentExecutionQueues
                .Where(q => q.Id == entry.Id)
                .ExecuteUpdateAsync(
                    s => s.SetProperty(q => q.Status, newStatus)
                          .SetProperty(q => q.UpdatedAt, DateTime.UtcNow),
                    context.CancellationToken);
        }
    }
}
