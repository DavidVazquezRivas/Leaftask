using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Modules.Agents.DrivenInfrastructure.KernelFactory;

public sealed class LlmLoggingFilter(ILogger<LlmLoggingFilter> logger) : IFunctionInvocationFilter
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
            logger.LogInformation(
                "LLM Request | Function: {Function}, MessageLength: {Length}b, Preview: {Preview}",
                context.Function.Name, userMessage.Length, LlmMessageExtractor.Truncate(userMessage));
        }

        try
        {
            await next(context);

            if (context.Result is not null)
            {
                string response = context.Result.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(response))
                {
                    logger.LogInformation(
                        "LLM Response | Function: {Function}, Length: {Length}b, Preview: {Preview}",
                        context.Function.Name, response.Length, LlmMessageExtractor.Truncate(response));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "LLM invocation failed for function '{Function}'", context.Function.Name);
            throw new InvalidOperationException(
                $"LLM invocation failed for function '{context.Function.Name}'", ex);
        }
    }

    private static bool IsChatCompletionFunction(KernelFunction function) =>
        function.PluginName == "ChatCompletionService" ||
        function.Name.Contains("GetChatMessageContent", StringComparison.OrdinalIgnoreCase) ||
        function.Name.Contains("completion", StringComparison.OrdinalIgnoreCase);
}
