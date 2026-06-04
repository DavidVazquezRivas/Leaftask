using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Modules.Agents.DrivenInfrastructure.ModelProvider;

namespace Modules.Agents.Application.Services;

public class AgentOrchestrator(SemanticKernelProvider KernelProvider)
{
    public async Task<string> ExecuteTaskAsync(string userPrompt, CancellationToken cancellationToken = default)
    {
        Kernel kernel = KernelProvider.CreateKernel();

        IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        PromptExecutionSettings settings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        ChatHistory chatHistory = [];
        chatHistory.AddSystemMessage($"""
                                      You are an autonomous AI project coordinator and team member within the platform.
                                      Your job is to assist users in managing their workspace, organizing tasks, etc.

                                      OPERATIONAL RULES:
                                      1. You can execute actions directly on behalf of the user using your available tools.
                                      2. Before creating, updating, or deleting any resource (roles, members, projects), you MUST ensure you have the correct GUID identifiers. If you do not have them in the conversation history, call the appropriate 'Get' or 'Search' tool first. Never guess or hallucinate a GUID.
                                      3. When a tool returns a [SUCCESS] message, assume the state has changed in the system and proceed with your reasoning.
                                      4. If a tool returns an [ERROR] message, explain the reason clearly to the user or attempt to correct your parameters if it was a formatting mistake.
                                      5. Always respond to the user in a professional, clear, corporate tone and in the language the user is using.

                                      Current Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC.
                                      """);

        chatHistory.AddUserMessage(userPrompt);

        ChatMessageContent response = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            settings,
            kernel,
            cancellationToken);

        return response.Content ?? "Task completed with no written response.";
    }
}
