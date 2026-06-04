using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Agents.Application.Services;
using Modules.Agents.DrivenInfrastructure.ModelProvider;
using Modules.Agents.DrivenInfrastructure.ModelProvider.ChatCompletion;

namespace Modules.Agents.DrivingInfrastructure.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddAgentsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddModelProvider(configuration);

        return services;
    }

    private static IServiceCollection AddModelProvider(this IServiceCollection services, IConfiguration configuration)
    {
        string providerType =
            configuration.GetSection("Modules:Agents:Provider").Get<string>()
                ?.ToLower(CultureInfo.DefaultThreadCurrentCulture) ??
            throw new InvalidOperationException("AI_PROVIDER environment variable is not set.");

        switch (providerType)
        {
            case "openai":
                services.AddSingleton<IChatCompletionConfigurator, OpenAiConfigurator>();
                break;
            default:
                throw new InvalidOperationException(
                    $"[AI_BOOTSTRAP] Unsupported AI provider type configured in 'Modules:Agents:Provider': '{providerType}'. Supported values are 'openai' or 'anthropic'.");
        }

        services.AddSingleton<SemanticKernelProvider>();
        services.AddScoped<AgentOrchestrator>();

        return services;
    }
}
