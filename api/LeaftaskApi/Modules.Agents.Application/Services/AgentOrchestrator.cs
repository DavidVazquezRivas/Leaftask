using System.Text.Json;
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
            IReadOnlyList<AgentExecutionMessage> existingMessages = execution.Mode == ExecutionMode.DirectQuery
                ? []
                : await messageRepository.GetByExecutionIdAsync(executionId, cancellationToken);

            ChatHistory chatHistory = BuildChatHistory(agent, execution, initialPayload, existingMessages);

            if (execution.Mode != ExecutionMode.DirectQuery && existingMessages.Count == 0)
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

            // DirectQuery executions don't persist messages — they are ephemeral by design
            if (execution.Mode != ExecutionMode.DirectQuery)
            {
                int nextSequence = existingMessages.Count == 0 ? 3 : existingMessages.Count + 1;
                await messageRepository.AddAsync(
                    new AgentExecutionMessage(Guid.NewGuid(), executionId, MessageRole.Assistant, content, null,
                        nextSequence),
                    cancellationToken);
                await messageRepository.SaveChangesAsync(cancellationToken);
            }

            // DirectQuery executions always complete — suspension is not allowed
            if (execution.Mode != ExecutionMode.DirectQuery && suspensionContext.ShouldSuspend)
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

    private static ChatHistory BuildChatHistory(Agent agent, AgentExecution execution,
        string initialPayload, IReadOnlyList<AgentExecutionMessage> existingMessages)
    {
        if (execution.Mode == ExecutionMode.DirectQuery)
            return BuildDirectQueryChatHistory(agent, execution.Payload);

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

    private static ChatHistory BuildDirectQueryChatHistory(Agent agent, string payload)
    {
        using JsonDocument doc = JsonDocument.Parse(payload);
        JsonElement root = doc.RootElement;

        Guid chatId = root.TryGetProperty("chatId", out JsonElement chatIdEl)
            ? chatIdEl.GetGuid()
            : Guid.Empty;
        string question = root.TryGetProperty("question", out JsonElement questionEl)
            ? questionEl.GetString() ?? string.Empty
            : string.Empty;
        string executionContext = root.TryGetProperty("executionContext", out JsonElement contextEl)
            ? contextEl.GetString() ?? string.Empty
            : string.Empty;

        ChatHistory history = [];
        history.AddSystemMessage(BuildDirectQuerySystemPrompt(agent, chatId, executionContext));
        history.AddUserMessage(question);
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
         ## EXECUTION CONTEXT
         - Agent ID:   {agent.Id}
         - Agent Name: {agent.Name}
         - Project ID: {agent.ProjectId}
         - Current Time (UTC): {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}

         All tool calls that require a projectId must use the Project ID above unless the task
         explicitly refers to a different project.

         ## IDENTITY AND PURPOSE
         {agent.SystemPrompt}

         ## EXECUTION PROTOCOL

         You are an autonomous agent. You act through tools. Your text output is a concise status
         report to the orchestration system — it is never shown directly to users as a message.

         **ACT, do not narrate.**
         ✗ Wrong: "I will now check the overdue tasks and notify the assignees."
         ✓ Correct: [call GetProjectWorkItems] → [call AddWorkItemComment per item] → report results.

         ### Rules (non-negotiable)

         1. **Tools first.** Any action (read, write, notify, update) must be executed via a tool call.
            Never describe an action you intend to do — perform it immediately.

         2. **Resolve IDs before acting.** Before calling Create, Update, or Delete on any resource,
            use Get or Search tools to obtain the exact GUIDs. Never invent or assume an identifier.

         3. **Tool results are final.**
            - [SUCCESS]: state has changed — move to the next step.
            - [ERROR]: diagnose the cause. Correct your parameters or explain why the action cannot be done.

         4. **Do not write user-facing messages as output.** Content intended for users must be
            delivered through tools. If you write it as plain text, it will be discarded.

         5. **Choosing the right communication tool:**
            - Use **SendChatMessage** when you need a conversational reply from a specific person
              (questions, status requests, approvals). Call FindOrCreateDirectChat first if you don't
              have the chatId. After sending, call SuspendWorkflow to wait for the reply.
            - Use **AddWorkItemComment** to log information, decisions, or status updates on a task.
              Do NOT use it to ask questions that require a human reply.

         6. **Suspend when awaiting a reply.** After SendChatMessage or any action requiring a human
            response, immediately call SuspendWorkflow with the event type and correlation IDs.
            In your final text output, document: the chatId used, what you asked, and what action
            to take when the reply arrives.

         7. **On resume.** When the last user message starts with "[RESUME — event: ..., id: CHAT_ID]",
            your SuspendWorkflow completed and the user has replied.
            a. Extract the chatId from the "id:" field in the resume tag.
            b. Immediately call **SendChatMessage(chatId, "brief acknowledgment")** so the user
               knows their reply was received (e.g. "Got it, I'll proceed now.").
            c. Look at your previous text summary (last assistant message) to determine the
               pending action, and execute it using tools.
            d. Do NOT re-do work already completed.

         8. **Final response.** After all tool calls are complete, write one short paragraph summarising
            what was accomplished. Use the same language as the triggering message. Nothing else.
         """;

    private static string BuildDirectQuerySystemPrompt(Agent agent, Guid chatId, string executionContext) =>
        $"""
         ## QUERY MODE
         You received a direct message from a user and must respond to it.

         ## INSTRUCTIONS
         1. Reply to the user using **SendChatMessage** with chatId="{chatId}".
         2. Be concise and helpful. Explain your current state if relevant.
         3. Do NOT call SuspendWorkflow — this is a single-turn response.
         4. Complete immediately after sending the reply.

         ## YOUR CURRENT STATE
         {executionContext}

         ## YOUR IDENTITY AND PURPOSE
         Agent ID:   {agent.Id}
         Agent Name: {agent.Name}
         Project ID: {agent.ProjectId}

         {agent.SystemPrompt}
         """;
}
