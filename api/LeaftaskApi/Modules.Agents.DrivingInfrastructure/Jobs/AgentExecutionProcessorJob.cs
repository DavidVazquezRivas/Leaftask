using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Agents.Application.Services;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;
using Quartz;

namespace Modules.Agents.DrivingInfrastructure.Jobs;

public sealed class AgentExecutionProcessorJob(
    IServiceScopeFactory scopeFactory,
    ILogger<AgentExecutionProcessorJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        IAgentExecutionRepository executionRepository =
            scope.ServiceProvider.GetRequiredService<IAgentExecutionRepository>();
        AgentOrchestrator orchestrator = scope.ServiceProvider.GetRequiredService<AgentOrchestrator>();

        int batchSize = context.MergedJobDataMap.GetIntValue("BatchSize");
        if (batchSize == 0) batchSize = 5;

        List<AgentExecution> pending = await executionRepository.GetPendingBatchAsync(batchSize,
            context.CancellationToken);

        if (pending.Count == 0)
            return;

        foreach (AgentExecution entry in pending)
        {
            try
            {
                await orchestrator.ExecuteAsync(entry.Id, entry.Agent, entry.Payload, context.CancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error executing agent {AgentId} for execution {ExecutionId}",
                    entry.Agent.Id, entry.Id);
            }
        }
    }
}
