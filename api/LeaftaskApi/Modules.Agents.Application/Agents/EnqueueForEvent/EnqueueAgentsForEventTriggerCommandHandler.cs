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

        foreach (Agent agent in agents)
        {
            AgentExecution entry = new(
                Guid.NewGuid(),
                command.Payload,
                ExecutionStatus.Pending,
                now,
                now,
                agent);

            await executionRepository.AddAsync(entry, cancellationToken);
        }

        await executionRepository.SaveChangesAsync(cancellationToken);
    }
}
