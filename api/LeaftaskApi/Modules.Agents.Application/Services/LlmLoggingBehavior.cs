using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using MsKernel = Microsoft.SemanticKernel.Kernel;

namespace Modules.Agents.Application.Services;

public sealed class LlmLoggingBehavior(ILogger<LlmLoggingBehavior> logger) : ILlmCallBehavior
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
            logger.LogInformation(
                "[LLM Request] | Messages: {Count}, Preview: {Preview}",
                chatHistory.Count, lastUserMessage);
        }

        Stopwatch sw = Stopwatch.StartNew();
        IReadOnlyList<ChatMessageContent> response = await next(cancellationToken);
        sw.Stop();

        string? firstContent = response.Count > 0 ? response[0].Content : null;
        if (!string.IsNullOrEmpty(firstContent))
        {
            logger.LogInformation(
                "[LLM Response] | Duration: {Duration}ms, Preview: {Preview}",
                sw.ElapsedMilliseconds, firstContent);
        }

        return response;
    }
}
