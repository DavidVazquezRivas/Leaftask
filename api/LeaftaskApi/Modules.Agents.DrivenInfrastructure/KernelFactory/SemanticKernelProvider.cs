using BuildingBlocks.DrivingInfrastructure.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Modules.Agents.Application.Kernel;
using Modules.Agents.Domain.Entities;

namespace Modules.Agents.DrivenInfrastructure.KernelFactory;

public sealed class SemanticKernelProvider(IServiceProvider serviceProvider) : IAgentKernelFactory
{
    private static readonly HttpClient AnthropicHttpClient = CreateAnthropicClient();

    public Kernel CreateKernel(Agent agent)
    {
        IKernelBuilder builder = Kernel.CreateBuilder();

        string providerName = agent.ModelConfig.Model.Provider.Name;
        string modelId = agent.ModelConfig.Model.Name;
        string apiKey = agent.ModelConfig.Model.Provider.Token;

        switch (providerName.ToUpperInvariant())
        {
            case "OPENAI":
                builder.AddOpenAIChatCompletion(modelId, apiKey);
                break;
            case "ANTHROPIC":
                builder.AddOpenAIChatCompletion(modelId, apiKey, httpClient: AnthropicHttpClient);
                break;
            default:
                throw new InvalidOperationException(
                    $"Unsupported model provider: '{providerName}'. Supported values are 'OpenAI' and 'Anthropic'.");
        }

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
