using BuildingBlocks.Application.Commands;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;

namespace Modules.Agents.Application.Agents.Resume;

public sealed class TryResumeAgentExecutionsCommandHandler(
    IAgentExecutionPendingEventRepository pendingEventRepository,
    IAgentExecutionRepository executionRepository,
    IAgentExecutionMessageRepository messageRepository)
    : ICommandHandler<TryResumeAgentExecutionsCommand>
{
    public async Task Handle(TryResumeAgentExecutionsCommand command, CancellationToken cancellationToken)
    {
        List<AgentExecutionPendingEvent> pendingEvents = await pendingEventRepository
            .GetUnresolvedAsync(command.EventType, command.CorrelationId, cancellationToken);

        if (pendingEvents.Count == 0)
            return;

        foreach (AgentExecutionPendingEvent pending in pendingEvents)
        {
            AgentExecution? execution = await executionRepository
                .GetByIdTrackedAsync(pending.ExecutionId, cancellationToken);

            if (execution is null)
                continue;

            IReadOnlyList<AgentExecutionMessage> existingMessages = await messageRepository
                .GetByExecutionIdAsync(pending.ExecutionId, cancellationToken);

            int nextSequence = existingMessages.Count + 1;

            await messageRepository.AddAsync(
                new AgentExecutionMessage(
                    Guid.NewGuid(),
                    pending.ExecutionId,
                    MessageRole.User,
                    command.NewMessage,
                    null,
                    nextSequence),
                cancellationToken);

            pending.Resolve();

            bool hasMoreUnresolved = await pendingEventRepository
                .HasUnresolvedAsync(pending.ExecutionId, cancellationToken);

            if (!hasMoreUnresolved)
                execution.ReQueue();
        }

        await executionRepository.SaveChangesAsync(cancellationToken);
    }
}
