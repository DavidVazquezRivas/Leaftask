using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Modules.Agents.DrivenInfrastructure.KernelFactory;

internal static class LlmMessageExtractor
{
    internal static string? ExtractUserMessage(KernelArguments? arguments)
    {
        if (arguments is null)
            return null;

        if (arguments.TryGetValue("messages", out object? messagesObj) && messagesObj is string msgStr)
            return ExtractLastUserMessage(msgStr);

        if (arguments.TryGetValue("prompt", out object? promptObj))
            return promptObj?.ToString();

        if (arguments.TryGetValue("userInput", out object? inputObj))
            return inputObj?.ToString();

        return null;
    }

    internal static string Truncate(string content, int maxLength = 150)
        => content.Length > maxLength ? $"{content[..maxLength]}..." : content;

    private static string? ExtractLastUserMessage(string messagesJson)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(messagesJson);
            JsonElement root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Array)
                return null;

            foreach (JsonElement msg in root.EnumerateArray().Reverse())
            {
                if (msg.TryGetProperty("role", out JsonElement roleEl) &&
                    roleEl.GetString()?.ToUpperInvariant() == "USER" &&
                    msg.TryGetProperty("content", out JsonElement contentEl))
                {
                    return contentEl.GetString();
                }
            }
        }
        catch (JsonException)
        {
            return null;
        }

        return null;
    }
}
