using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Modules.Agents.Application.Bootstrap;
using Modules.Agents.Domain.Entities.Model;
using Modules.Agents.Domain.Repositories;

namespace Modules.Agents.DrivenInfrastructure.Bootstrap;

public sealed class BootstrapAgentService(
    IModelRepository modelRepository,
    IConfiguration configuration) : IBootstrapAgentService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<BootstrapAgentResult?> GenerateAsync(
        string instructions,
        IReadOnlyList<AvailableModelDto> availableModels,
        IReadOnlyList<string> availableEventTypes,
        CancellationToken cancellationToken = default)
    {
        Model? bootstrapModel = await ResolveBootstrapModelAsync(availableModels, cancellationToken);
        if (bootstrapModel is null)
            return null;

        IKernelBuilder builder = Kernel.CreateBuilder();
        ConfigureChatCompletion(builder, bootstrapModel);
        Kernel kernel = builder.Build();

        IChatCompletionService chat = kernel.GetRequiredService<IChatCompletionService>();

        string modelsJson = JsonSerializer.Serialize(
            availableModels.Select(m => new { id = m.Id, name = m.Name, costPer1kTokens = m.Cost }),
            JsonOptions);

        string eventsJson = JsonSerializer.Serialize(availableEventTypes, JsonOptions);

        ChatHistory history = [];
        history.AddSystemMessage($$"""
            You are an expert AI agent configuration assistant for a project management platform.
            Given a task description, output ONLY a valid JSON object matching exactly this schema — no markdown, no explanation:

            {
              "systemPrompt": string,
              "modelId": string (one of the provided model GUIDs),
              "temperature": number 0–1,
              "maxTokens": integer 256–8192,
              "timeTriggers": [ { "name": string, "cronExpression": string (Quartz cron, 6 fields), "timeZone": string (IANA) } ],
              "eventTriggers": [ { "eventType": string (one of the provided event types), "userPrompt": string } ]
            }

            ## Rules

            ### Model selection
            - Choose the most cost-effective model suitable for the task complexity.
            - Analytical or multi-step tasks warrant higher-capability models.

            ### systemPrompt — CRITICAL
            The agent operates autonomously and communicates exclusively through tool calls.
            The systemPrompt you write must:
            - State the agent's concrete objective in one sentence.
            - List the sequential steps the agent must follow, referencing specific tool names
              (GetProjectWorkItems, GetProjectMembers, SendChatMessage, FindOrCreateDirectChat,
               AddWorkItemComment, UpdateWorkItem, SearchUsers, etc.).
            - Explicitly forbid the agent from narrating or describing actions — it must call tools directly.
            - Specify what the agent should do if a required resource is not found.

            COMMUNICATION TOOL SELECTION RULE — apply this when writing systemPrompt:
            - Use SendChatMessage (preceded by FindOrCreateDirectChat if chatId unknown) when the goal
              is to ask a question to a specific person or start a conversation that requires their reply.
              After SendChatMessage, always call SuspendWorkflow to wait for the response.
            - Use AddWorkItemComment to log information, status updates, or decisions on a task.
              Do NOT instruct the agent to use AddWorkItemComment for asking questions to users.

            BAD systemPrompt (conversational, vague, no tool references):
              "You are a helpful assistant that monitors tasks and reminds team members about deadlines."

            BAD systemPrompt (uses AddWorkItemComment to ask questions):
              "...Call AddWorkItemComment to ask the assignee for their current status..."

            GOOD systemPrompt (action-oriented, tool-explicit, correct communication tools):
              "Request a status update from each team member every time you are triggered:
               1. Call GetProjectMembers to get all active members.
               2. For each member, call GetProjectWorkItems filtering by assignee to find their active task.
               3. Call FindOrCreateDirectChat with the member's userId, then call SendChatMessage asking
                  for their progress update on the task.
               4. Call SuspendWorkflow(eventType='chat.message_sent', correlationIds=<chatId>) to wait
                  for their reply.
               5. When resumed with the reply, call AddWorkItemComment on their task to record the
                  reported status.
               6. Never write messages as plain text output — always use SendChatMessage or AddWorkItemComment."

            ### eventTriggers — userPrompt field
            The userPrompt is the instruction sent to the agent when the event fires.
            It must be a direct, imperative command describing what to do with the event data, not a question.
            BAD:  "A new work item was created. What should I do?"
            GOOD: "A new work item was just created in your project. Review it, assign it to the correct
                   team member based on skill area, and set the appropriate priority and due date."

            ### Triggers
            - Use timeTriggers for scheduled or recurring tasks (Quartz 6-field cron, e.g. "0 0 9 * * ?").
            - Use eventTriggers to react to platform events.
            - Both arrays may be empty if the agent is invoked only on demand.

            Available models: {{modelsJson}}
            Available event types: {{eventsJson}}
            """);

        history.AddUserMessage($"Configure an agent for the following task: {instructions}");

        ChatMessageContent response = await chat.GetChatMessageContentAsync(
            history,
            cancellationToken: cancellationToken);

        return ParseResult(response.Content);
    }

    private async Task<Model?> ResolveBootstrapModelAsync(
        IReadOnlyList<AvailableModelDto> availableModels,
        CancellationToken cancellationToken)
    {
        string? configuredId = configuration["Modules:Agents:Bootstrap:ModelId"];

        if (!string.IsNullOrWhiteSpace(configuredId) && Guid.TryParse(configuredId, out Guid modelId))
        {
            Model? configured = await modelRepository.GetByIdAsync(modelId, cancellationToken);
            if (configured is not null)
                return configured;
        }

        Guid cheapestId = availableModels.OrderBy(m => m.Cost).First().Id;
        return await modelRepository.GetByIdAsync(cheapestId, cancellationToken);
    }

    private static void ConfigureChatCompletion(IKernelBuilder builder, Model model)
    {
        string providerName = model.Provider.Name;
        string apiKey = model.Provider.Token;
        string modelId = model.Name;

        switch (providerName.ToUpperInvariant())
        {
            case "OPENAI":
                builder.AddOpenAIChatCompletion(modelId, apiKey);
                break;
            case "ANTHROPIC":
                builder.AddOpenAIChatCompletion(modelId, apiKey, httpClient: KernelFactory.SemanticKernelProvider.AnthropicHttpClient);
                break;
            default:
                throw new InvalidOperationException($"Unsupported provider for bootstrap: {providerName}");
        }
    }

    private static BootstrapAgentResult? ParseResult(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return null;

        string json = content.Trim();

        if (json.StartsWith("```", StringComparison.Ordinal))
        {
            int start = json.IndexOf('{', StringComparison.Ordinal);
            int end = json.LastIndexOf('}');
            if (start >= 0 && end > start)
                json = json[start..(end + 1)];
        }

        try
        {
            BootstrapJson? parsed = JsonSerializer.Deserialize<BootstrapJson>(json, JsonOptions);
            if (parsed is null || string.IsNullOrWhiteSpace(parsed.SystemPrompt))
                return null;

            return new BootstrapAgentResult(
                parsed.SystemPrompt,
                parsed.ModelId,
                Math.Clamp(parsed.Temperature, 0.0, 1.0),
                Math.Clamp(parsed.MaxTokens, 256, 8192),
                parsed.TimeTriggers?.Select(t => new BootstrapTimeTrigger(t.Name, t.CronExpression, t.TimeZone)).ToList() ?? [],
                parsed.EventTriggers?.Select(e => new BootstrapEventTrigger(e.EventType, e.UserPrompt)).ToList() ?? []);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private sealed record BootstrapJson(
        string SystemPrompt,
        Guid ModelId,
        double Temperature,
        int MaxTokens,
        List<BootstrapJsonTimeTrigger>? TimeTriggers,
        List<BootstrapJsonEventTrigger>? EventTriggers);

    private sealed record BootstrapJsonTimeTrigger(string Name, string CronExpression, string TimeZone);
    private sealed record BootstrapJsonEventTrigger(string EventType, string UserPrompt);
}
