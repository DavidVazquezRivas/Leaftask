using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Entities.AgentTriggers;
using Modules.Agents.Domain.Errors;
using Modules.Agents.Domain.Repositories;

namespace Modules.Agents.Application.Agents.Delete;

public sealed class DeleteAgentCommandHandler(
    IAgentRepository agentRepository,
    IAgentScheduler agentScheduler)
    : ICommandHandler<DeleteAgentCommand, Result>
{
    public async Task<Result> Handle(DeleteAgentCommand command, CancellationToken cancellationToken)
    {
        Agent? agent = await agentRepository.GetByIdTrackedAsync(command.AgentId, cancellationToken);
        if (agent is null || agent.ProjectId != command.ProjectId)
            return Result.Failure(AgentErrors.AgentNotFound);

        foreach (AgentTimeTrigger trigger in agent.TimeTriggers)
            await agentScheduler.UnscheduleTimeTriggerAsync(trigger.Id, cancellationToken);

        agent.Delete();

        await agentRepository.RemoveAsync(agent, cancellationToken);
        await agentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
