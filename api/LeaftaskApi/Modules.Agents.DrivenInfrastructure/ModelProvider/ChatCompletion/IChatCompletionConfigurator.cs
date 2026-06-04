using Microsoft.SemanticKernel;

namespace Modules.Agents.DrivenInfrastructure.ModelProvider.ChatCompletion;

public interface IChatCompletionConfigurator
{
    void Configure(IKernelBuilder builder);
}
