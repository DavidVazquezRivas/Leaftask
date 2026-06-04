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
            You are an expert AI agent configuration assistant.
            Given a description of what an agent should do, output ONLY a valid JSON object with exactly this schema:
            {
              "systemPrompt": string,
              "modelId": string (one of the provided model GUIDs),
              "temperature": number between 0 and 1,
              "maxTokens": integer between 256 and 8192,
              "timeTriggers": [ { "name": string, "cronExpression": string (quartz cron), "timeZone": string (IANA) } ],
              "eventTriggers": [ { "eventType": string (one of the provided event types), "userPrompt": string } ]
            }
            Rules:
            - timeTriggers and eventTriggers may be empty arrays if not needed.
            - Choose the most cost-effective model appropriate for the task.
            - The systemPrompt should be a clear, detailed prompt guiding the agent.
            - Do NOT include any explanation or text outside the JSON.

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
