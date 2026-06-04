using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace Modules.Agents.DrivenInfrastructure.ModelProvider.ChatCompletion;

public sealed class OpenAiConfigurator(IConfiguration configuration) : IChatCompletionConfigurator
{
    public void Configure(IKernelBuilder builder)
    {
        OpenAiOptions options = configuration.GetSection("Modules:Agents:OpenAi").Get<OpenAiOptions>()
                                ?? new OpenAiOptions();

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new InvalidOperationException(
                "[AI_CONFIG] OpenAI ApiKey is missing. Please check your configuration under 'Modules:Agents:OpenAi:ApiKey'.");
        }

        builder.AddOpenAIChatCompletion(options.ModelId, options.ApiKey);
    }
}

public sealed record OpenAiOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public string ModelId { get; init; } = "gpt-4o-mini";
}
