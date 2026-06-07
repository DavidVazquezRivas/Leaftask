using BuildingBlocks.DrivingInfrastructure.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Modules.Agents.Application.Kernel;
using Modules.Agents.Application.Services;
using Modules.Agents.Domain.Entities;
using Modules.Agents.DrivenInfrastructure.KernelFactory;

namespace Modules.Agents.DrivenInfrastructure.KernelFactory;

public sealed class SemanticKernelProvider(
    IServiceProvider serviceProvider,
    PromptInjectionDetector injectionDetector,
    ILogger<LlmLoggingFilter> loggingFilterLogger,
    ILogger<LlmPromptInjectionFilter> securityFilterLogger) : IAgentKernelFactory
{
    internal static readonly HttpClient AnthropicHttpClient = CreateAnthropicClient();

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

        Kernel kernel = builder.Build();

        kernel.FunctionInvocationFilters.Add(new LlmPromptInjectionFilter(injectionDetector, securityFilterLogger));
        kernel.FunctionInvocationFilters.Add(new LlmLoggingFilter(loggingFilterLogger));

        return kernel;
    }

    private static HttpClient CreateAnthropicClient()
    {
        HttpClient client = new() { BaseAddress = new Uri(AnthropicApiBase) };
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        return client;
    }

    private const string AnthropicApiBase = "https://api.anthropic.com/v1/";
}
