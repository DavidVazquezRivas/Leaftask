using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;
using Quartz;

namespace Modules.Agents.DrivingInfrastructure.Jobs;

public sealed class AgentExecutionTimeoutJob(
    IServiceScopeFactory scopeFactory,
    ILogger<AgentExecutionTimeoutJob> logger) : IJob
{
    private static readonly TimeSpan DefaultThreshold = TimeSpan.FromHours(2);

    public async Task Execute(IJobExecutionContext context)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        IAgentExecutionRepository executionRepository =
            scope.ServiceProvider.GetRequiredService<IAgentExecutionRepository>();
        IAgentExecutionMessageRepository messageRepository =
            scope.ServiceProvider.GetRequiredService<IAgentExecutionMessageRepository>();
        IAgentExecutionPendingEventRepository pendingEventRepository =
            scope.ServiceProvider.GetRequiredService<IAgentExecutionPendingEventRepository>();

        int thresholdHours = context.MergedJobDataMap.GetIntValue("ThresholdHours");
        TimeSpan threshold = thresholdHours > 0 ? TimeSpan.FromHours(thresholdHours) : DefaultThreshold;

        List<AgentExecution> timedOut =
            await executionRepository.GetTimedOutSuspendedAsync(threshold, context.CancellationToken);

        if (timedOut.Count == 0)
            return;

        logger.LogInformation("[AgentExecutionTimeout] Found {Count} timed-out suspended executions", timedOut.Count);

        foreach (AgentExecution execution in timedOut)
        {
            await pendingEventRepository.ResolveAllForExecutionAsync(execution.Id, context.CancellationToken);

            IReadOnlyList<AgentExecutionMessage> existing =
                await messageRepository.GetByExecutionIdAsync(execution.Id, context.CancellationToken);

            int nextSequence = existing.Count + 1;
            AgentExecutionMessage timeoutMessage = new(
                Guid.NewGuid(),
                execution.Id,
                MessageRole.User,
                "TIMEOUT: No response received. Re-evaluate your pending tasks. " +
                "If you were waiting for a reply that has not arrived, try re-sending the message or escalate.",
                null,
                nextSequence);

            await messageRepository.AddAsync(timeoutMessage, context.CancellationToken);
            execution.ReQueue();

            logger.LogInformation(
                "[AgentExecutionTimeout] Re-queued execution {ExecutionId} for agent {AgentId}",
                execution.Id, execution.AgentId);
        }

        await messageRepository.SaveChangesAsync(context.CancellationToken);
        await executionRepository.SaveChangesAsync(context.CancellationToken);
    }
}
