using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using MsKernel = Microsoft.SemanticKernel.Kernel;

namespace Modules.Agents.Application.Services;

public sealed class LlmPromptInjectionBehavior(
    PromptInjectionDetector injectionDetector,
    ILogger<LlmPromptInjectionBehavior> logger) : ILlmCallBehavior
{
    public async Task<IReadOnlyList<ChatMessageContent>> ExecuteAsync(
        ChatHistory chatHistory,
        PromptExecutionSettings? settings,
        MsKernel? kernel,
        Func<CancellationToken, Task<IReadOnlyList<ChatMessageContent>>> next,
        CancellationToken cancellationToken)
    {
        string? lastUserMessage = chatHistory.LastOrDefault(m => m.Role == AuthorRole.User)?.Content;

        if (!string.IsNullOrEmpty(lastUserMessage))
        {
            PromptInjectionResult result = await injectionDetector.DetectAsync(lastUserMessage);

            if (result.IsInjection)
            {
                float confidencePct = result.Confidence * 100;
                logger.LogError(
                    "Prompt injection detected ({Phase}, confidence: {Confidence:F1}%): {Preview}",
                    result.Phase, confidencePct, Truncate(lastUserMessage));

                throw new InvalidOperationException(
                    $"Request blocked: prompt injection detected ({result.Phase}, {result.Confidence:P} confidence)");
            }
        }

        return await next(cancellationToken);
    }

    private static string Truncate(string s, int max = 150)
        => s.Length > max ? $"{s[..max]}..." : s;
}
