using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Agents.Domain.Entities.Queue;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Quartz;

namespace Modules.Agents.DrivingInfrastructure.Jobs;

public sealed class AgentTimeTriggerExecutionJob(
    IServiceScopeFactory scopeFactory,
    ILogger<AgentTimeTriggerExecutionJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        string? agentIdStr = context.MergedJobDataMap.GetString("agentId");
        string? triggerIdStr = context.MergedJobDataMap.GetString("triggerId");

        if (!Guid.TryParse(agentIdStr, out Guid agentId) || !Guid.TryParse(triggerIdStr, out Guid triggerId))
        {
            logger.LogWarning("AgentTimeTriggerExecutionJob: invalid agentId={AgentId} or triggerId={TriggerId}",
                agentIdStr, triggerIdStr);
            return;
        }

        using IServiceScope scope = scopeFactory.CreateScope();
        AgentsDbContext dbContext = scope.ServiceProvider.GetRequiredService<AgentsDbContext>();

        Domain.Entities.Agent? agent = await dbContext.Agents
            .Include(a => a.TimeTriggers)
            .FirstOrDefaultAsync(a => a.Id == agentId, context.CancellationToken);

        if (agent is null)
        {
            logger.LogWarning("AgentTimeTriggerExecutionJob: agent {AgentId} not found", agentId);
            return;
        }

        DateTime now = DateTime.UtcNow;
        AgentExecutionQueue entry = new(Guid.NewGuid(), "{}", QueueStatus.Pending, now, now, agent);
        await dbContext.AgentExecutionQueues.AddAsync(entry, context.CancellationToken);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Enqueued time-triggered execution for agent {AgentId} (trigger {TriggerId})",
            agentId, triggerId);
    }
}
