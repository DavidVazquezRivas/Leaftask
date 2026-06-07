using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using MsKernel = Microsoft.SemanticKernel.Kernel;

namespace Modules.Agents.Application.Services;

public interface ILlmCallBehavior
{
    Task<IReadOnlyList<ChatMessageContent>> ExecuteAsync(
        ChatHistory chatHistory,
        PromptExecutionSettings? settings,
        MsKernel? kernel,
        Func<CancellationToken, Task<IReadOnlyList<ChatMessageContent>>> next,
        CancellationToken cancellationToken);
}
