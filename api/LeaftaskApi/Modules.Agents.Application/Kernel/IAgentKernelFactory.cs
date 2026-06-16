using Modules.Agents.Domain.Entities;

namespace Modules.Agents.Application.Kernel;

public interface IAgentKernelFactory
{
    Microsoft.SemanticKernel.Kernel CreateKernel(Agent agent);
}
