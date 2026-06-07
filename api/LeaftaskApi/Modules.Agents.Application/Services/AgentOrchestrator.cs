using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Modules.Agents.Application.Kernel;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;

namespace Modules.Agents.Application.Services;

public sealed class AgentOrchestrator(
    IAgentKernelFactory kernelFactory,
    IAgentExecutionRepository executionRepository,
    IAgentExecutionMessageRepository messageRepository,
    IAgentExecutionPendingEventRepository pendingEventRepository,
    AgentSuspensionContext suspensionContext,
    AgentExecutionContext executionContext)
{
    public async Task ExecuteAsync(
        Guid executionId,
        Agent agent,
        string initialPayload,
        CancellationToken cancellationToken = default)
    {
        AgentExecution? execution = await executionRepository.GetByIdTrackedAsync(executionId, cancellationToken);
        if (execution is null)
            return;

        executionContext.Activate(agent.Id, agent.ProjectId);
        execution.Start();
        await executionRepository.SaveChangesAsync(cancellationToken);

        try
        {
            IReadOnlyList<AgentExecutionMessage> existingMessages =
                await messageRepository.GetByExecutionIdAsync(executionId, cancellationToken);

            ChatHistory chatHistory = BuildChatHistory(agent, initialPayload, existingMessages);

            if (existingMessages.Count == 0)
                await PersistInitialMessages(executionId, agent, initialPayload, cancellationToken);

            Microsoft.SemanticKernel.Kernel kernel = kernelFactory.CreateKernel(agent);
            IChatCompletionService chat = kernel.GetRequiredService<IChatCompletionService>();

            PromptExecutionSettings settings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            ChatMessageContent response = await chat.GetChatMessageContentAsync(
                chatHistory, settings, kernel, cancellationToken);

            string content = response.Content ?? string.Empty;

            int nextSequence = existingMessages.Count == 0 ? 3 : existingMessages.Count + 1;
            await messageRepository.AddAsync(
                new AgentExecutionMessage(Guid.NewGuid(), executionId, MessageRole.Assistant, content, null, nextSequence),
                cancellationToken);
            await messageRepository.SaveChangesAsync(cancellationToken);

            if (suspensionContext.ShouldSuspend)
            {
                List<AgentExecutionPendingEvent> pendingEvents = suspensionContext.CorrelationIds
                    .Select(correlationId => new AgentExecutionPendingEvent(
                        Guid.NewGuid(), executionId, suspensionContext.EventType, correlationId))
                    .ToList();

                await pendingEventRepository.AddRangeAsync(pendingEvents, cancellationToken);
                execution.Suspend();
            }
            else
            {
                execution.Complete();
            }

            await executionRepository.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            execution.Fail();
            await executionRepository.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    private static ChatHistory BuildChatHistory(Agent agent, string initialPayload,
        IReadOnlyList<AgentExecutionMessage> existingMessages)
    {
        ChatHistory history = [];

        if (existingMessages.Count == 0)
        {
            history.AddSystemMessage(BuildSystemPrompt(agent));
            history.AddUserMessage(initialPayload);
            return history;
        }

        foreach (AgentExecutionMessage msg in existingMessages.OrderBy(m => m.SequenceNumber))
        {
            switch (msg.Role)
            {
                case MessageRole.System:
                    history.AddSystemMessage(msg.Content);
                    break;
                case MessageRole.User:
                    history.AddUserMessage(msg.Content);
                    break;
                case MessageRole.Assistant:
                    history.AddAssistantMessage(msg.Content);
                    break;
            }
        }

        return history;
    }

    private async Task PersistInitialMessages(Guid executionId, Agent agent, string initialPayload,
        CancellationToken cancellationToken)
    {
        AgentExecutionMessage systemMsg = new(Guid.NewGuid(), executionId, MessageRole.System,
            BuildSystemPrompt(agent), null, 1);
        AgentExecutionMessage userMsg = new(Guid.NewGuid(), executionId, MessageRole.User,
            initialPayload, null, 2);

        await messageRepository.AddRangeAsync([systemMsg, userMsg], cancellationToken);
        await messageRepository.SaveChangesAsync(cancellationToken);
    }

    private static string BuildSystemPrompt(Agent agent) =>
        $"""
         {agent.SystemPrompt}

         OPERATIONAL RULES:
         1. You can execute actions directly on behalf of the user using your available tools.
         2. Before creating, updating, or deleting any resource, ensure you have correct GUID identifiers. Use Get/Search tools first - never guess or hallucinate a GUID.
         3. When a tool returns [SUCCESS], assume state has changed and proceed.
         4. If a tool returns [ERROR], explain the reason or correct your parameters.
         5. After sending a message to a user that requires their reply, immediately call SuspendWorkflow to wait for their response.
         6. Always respond in a professional, clear tone in the language the user is using.

         Current Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC.
         """;
}
