using BuildingBlocks.Application.Commands;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;

namespace Modules.Agents.Application.Agents.Resume;

public sealed class TryResumeAgentExecutionsCommandHandler(
    IAgentExecutionPendingEventRepository pendingEventRepository,
    IAgentExecutionRepository executionRepository,
    IAgentExecutionMessageRepository messageRepository)
    : ICommandHandler<TryResumeAgentExecutionsCommand, HashSet<Guid>>
{
    public async Task<HashSet<Guid>> Handle(TryResumeAgentExecutionsCommand command, CancellationToken cancellationToken)
    {
        List<AgentExecutionPendingEvent> matchingPendingEvents = await pendingEventRepository
            .GetUnresolvedAsync(command.EventType, command.CorrelationId, cancellationToken);

        if (matchingPendingEvents.Count == 0)
            return [];

        HashSet<Guid> resumedAgentIds = [];

        foreach (AgentExecutionPendingEvent pending in matchingPendingEvents)
        {
            AgentExecution? execution = await executionRepository
                .GetByIdTrackedAsync(pending.ExecutionId, cancellationToken);

            if (execution is null)
                continue;

            // Load ALL unresolved events for this execution into the tracker.
            // EF identity map ensures the objects in this list are the same references
            // as those already tracked (including 'pending'), so in-memory changes
            // to 'pending' are immediately visible here.
            List<AgentExecutionPendingEvent> allUnresolvedForExecution = await pendingEventRepository
                .GetAllUnresolvedByExecutionAsync(pending.ExecutionId, cancellationToken);

            IReadOnlyList<AgentExecutionMessage> existingMessages = await messageRepository
                .GetByExecutionIdAsync(pending.ExecutionId, cancellationToken);

            int nextSequence = existingMessages.Count + 1;

            string resumeMessage = $"[RESUME — event: {command.EventType}, id: {command.CorrelationId}]\n{command.NewMessage}";

            await messageRepository.AddAsync(
                new AgentExecutionMessage(
                    Guid.NewGuid(),
                    pending.ExecutionId,
                    MessageRole.User,
                    resumeMessage,
                    null,
                    nextSequence),
                cancellationToken);

            pending.Resolve();

            // Check in memory: 'pending' and its entry in allUnresolvedForExecution are the
            // same object reference, so this correctly reflects the just-resolved state.
            if (allUnresolvedForExecution.All(p => p.IsResolved))
            {
                execution.ReQueue();
                resumedAgentIds.Add(execution.AgentId);
            }
        }

        await executionRepository.SaveChangesAsync(cancellationToken);

        return resumedAgentIds;
    }
}
