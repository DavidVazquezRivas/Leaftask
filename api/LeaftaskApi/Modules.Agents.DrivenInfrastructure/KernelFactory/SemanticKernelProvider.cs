using BuildingBlocks.DrivingInfrastructure.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Modules.Agents.Application.Kernel;
using Modules.Agents.Application.Services;
using Modules.Agents.Domain.Entities;

namespace Modules.Agents.DrivenInfrastructure.KernelFactory;

public sealed class SemanticKernelProvider(IServiceProvider serviceProvider) : IAgentKernelFactory
{
    internal static readonly HttpClient AnthropicHttpClient = CreateAnthropicClient();

    public Kernel CreateKernel(Agent agent)
    {
        IKernelBuilder builder = Kernel.CreateBuilder();

        string providerName = agent.ModelConfig.Model.Provider.Name;
        string modelId = agent.ModelConfig.Model.Name;
        string apiKey = agent.ModelConfig.Model.Provider.Token;

        IChatCompletionService baseService = providerName.ToUpperInvariant() switch
        {
            "OPENAI" => new OpenAIChatCompletionService(modelId, apiKey),
            "ANTHROPIC" => new OpenAIChatCompletionService(modelId, apiKey, httpClient: AnthropicHttpClient),
            _ => throw new InvalidOperationException(
                $"Unsupported model provider: '{providerName}'. Supported values are 'OpenAI' and 'Anthropic'.")
        };

        // Build the LLM call pipeline from all registered behaviors (same pattern as MediatR)
        IEnumerable<ILlmCallBehavior> behaviors = serviceProvider.GetServices<ILlmCallBehavior>();
        IChatCompletionService pipeline = LlmCallPipelineBuilder.Build(baseService, behaviors);

        builder.Services.AddSingleton(pipeline);

        IEnumerable<IAiTool> allTools = serviceProvider.GetServices<IAiTool>();
        foreach (IAiTool tool in allTools)
        {
            builder.Plugins.AddFromObject(tool);
        }

        return builder.Build();
    }

    private static HttpClient CreateAnthropicClient()
    {
        HttpClient client = new() { BaseAddress = new Uri(AnthropicApiBase) };
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        return client;
    }

    private const string AnthropicApiBase = "https://api.anthropic.com/v1/";
}
