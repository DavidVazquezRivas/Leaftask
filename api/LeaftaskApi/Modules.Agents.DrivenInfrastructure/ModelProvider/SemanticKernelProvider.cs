using BuildingBlocks.DrivingInfrastructure.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Modules.Agents.DrivenInfrastructure.ModelProvider.ChatCompletion;

namespace Modules.Agents.DrivenInfrastructure.ModelProvider;

public sealed class SemanticKernelProvider(
    IServiceProvider serviceProvider,
    IChatCompletionConfigurator configurator)
{
    public Kernel CreateKernel()
    {
        IKernelBuilder builder = Kernel.CreateBuilder();

        configurator.Configure(builder);

        IEnumerable<IAiTool> allTools = serviceProvider.GetServices<IAiTool>();
        foreach (IAiTool tool in allTools)
        {
            builder.Plugins.AddFromObject(tool);
        }

        return builder.Build();
    }
}
