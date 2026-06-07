using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Modules.Agents.Application.Services;

namespace Modules.Agents.DrivenInfrastructure.KernelFactory;

public sealed class LlmPromptInjectionFilter(
    PromptInjectionDetector injectionDetector,
    ILogger<LlmPromptInjectionFilter> logger) : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(
        FunctionInvocationContext context,
        Func<FunctionInvocationContext, Task> next)
    {
        if (!IsChatCompletionFunction(context.Function))
        {
            await next(context);
            return;
        }

        if (LlmMessageExtractor.ExtractUserMessage(context.Arguments) is { } userMessage)
        {
            PromptInjectionResult result = await injectionDetector.DetectAsync(userMessage);

            if (result.IsInjection)
            {
                float confidencePercent = result.Confidence * 100;
                logger.LogError(
                    "Prompt injection detected ({Phase}, confidence: {Confidence:F1}%): {Message}",
                    result.Phase, confidencePercent, LlmMessageExtractor.Truncate(userMessage));

                throw new InvalidOperationException(
                    $"Potential prompt injection detected with {result.Confidence:P} confidence");
            }
        }

        await next(context);
    }

    private static bool IsChatCompletionFunction(KernelFunction function) =>
        function.PluginName == "ChatCompletionService" ||
        function.Name.Contains("GetChatMessageContent", StringComparison.OrdinalIgnoreCase) ||
        function.Name.Contains("completion", StringComparison.OrdinalIgnoreCase);
}
