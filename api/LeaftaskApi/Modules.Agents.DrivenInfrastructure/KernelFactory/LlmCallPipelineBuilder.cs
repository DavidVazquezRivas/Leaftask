using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Modules.Agents.Application.Services;

namespace Modules.Agents.DrivenInfrastructure.KernelFactory;

public static class LlmCallPipelineBuilder
{
    // Wraps baseService with all behaviors in registration order (first = outermost).
    public static IChatCompletionService Build(
        IChatCompletionService baseService,
        IEnumerable<ILlmCallBehavior> behaviors)
    {
        IChatCompletionService current = baseService;

        // Reverse so that the first registered behavior ends up outermost (called first)
        foreach (ILlmCallBehavior behavior in behaviors.Reverse())
        {
            current = new BehaviorAdapter(current, behavior);
        }

        return current;
    }

    private sealed class BehaviorAdapter(
        IChatCompletionService inner,
        ILlmCallBehavior behavior) : IChatCompletionService
    {
        public IReadOnlyDictionary<string, object?> Attributes => inner.Attributes;

        public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
            => behavior.ExecuteAsync(
                chatHistory,
                executionSettings,
                kernel,
                ct => inner.GetChatMessageContentsAsync(chatHistory, executionSettings, kernel, ct),
                cancellationToken);

        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
            => inner.GetStreamingChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken);
    }
}
