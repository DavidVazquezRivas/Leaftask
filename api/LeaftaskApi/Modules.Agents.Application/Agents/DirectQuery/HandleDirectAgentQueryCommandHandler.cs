using System.Text.Json;
using BuildingBlocks.Application.Commands;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;

namespace Modules.Agents.Application.Agents.DirectQuery;

public sealed class HandleDirectAgentQueryCommandHandler(
    IAgentRepository agentRepository,
    IAgentExecutionRepository executionRepository) : ICommandHandler<HandleDirectAgentQueryCommand>
{
    public async Task Handle(HandleDirectAgentQueryCommand command, CancellationToken cancellationToken)
    {
        if (await agentRepository.GetByIdAsync(command.AgentId, cancellationToken) is null)
            return;

        AgentExecution? activeExecution =
            await executionRepository.GetActiveRegularForAgentAsync(command.AgentId, cancellationToken);

        string executionContext = BuildExecutionContext(activeExecution);

        string payload = JsonSerializer.Serialize(new
        {
            chatId = command.ChatId,
            question = command.Message,
            executionContext
        });

        DateTime now = DateTime.UtcNow;
        AgentExecution entry = new(
            Guid.NewGuid(),
            payload,
            ExecutionStatus.Pending,
            now,
            now,
            command.AgentId,
            ExecutionMode.DirectQuery);

        await executionRepository.AddAsync(entry, cancellationToken);
        await executionRepository.SaveChangesAsync(cancellationToken);
    }

    private static string BuildExecutionContext(AgentExecution? execution)
    {
        if (execution is null)
            return "Currently idle with no active tasks.";

        if (execution.Status == ExecutionStatus.Suspended)
        {
            if (execution.PendingEvents.Count == 0)
                return "Currently suspended, waiting to resume.";

            IEnumerable<string> waitingFor = execution.PendingEvents
                .Where(pe => !pe.IsResolved)
                .Select(pe => $"{pe.EventType} (id: {pe.CorrelationId})");

            return $"Currently suspended, waiting for: {string.Join(", ", waitingFor)}.";
        }

        return execution.Status == ExecutionStatus.Processing
            ? "Currently processing a task."
            : "Currently has a task queued and waiting to start.";
    }
}
