using BuildingBlocks.Application.Commands;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;

namespace Modules.Agents.Application.Agents.EnqueueForEvent;

public sealed class EnqueueAgentsForEventTriggerCommandHandler(
    IAgentRepository agentRepository,
    IAgentExecutionRepository executionRepository)
    : ICommandHandler<EnqueueAgentsForEventTriggerCommand>
{
    public async Task Handle(EnqueueAgentsForEventTriggerCommand command, CancellationToken cancellationToken)
    {
        IReadOnlyList<Agent> agents = await agentRepository.GetByEventTriggerAsync(command.EventType, cancellationToken);

        if (agents.Count == 0)
            return;

        DateTime now = DateTime.UtcNow;

        foreach (Guid agentId in agents.Select(a => a.Id))
        {
            if (await executionRepository.HasActiveRegularExecutionAsync(agentId, cancellationToken))
                continue;

            AgentExecution entry = new(
                Guid.NewGuid(),
                command.Payload,
                ExecutionStatus.Pending,
                now,
                now,
                agentId);

            await executionRepository.AddAsync(entry, cancellationToken);
        }

        await executionRepository.SaveChangesAsync(cancellationToken);
    }
}
