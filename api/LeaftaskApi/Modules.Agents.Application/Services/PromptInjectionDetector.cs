using Microsoft.Extensions.Logging;

namespace Modules.Agents.Application.Services;

public sealed class PromptInjectionDetector(
    ILogger<PromptInjectionDetector> logger)
{
    private static readonly IReadOnlySet<string> SuspiciousKeywords =
    new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "ignore previous instructions",
        "disregard your instructions",
        "forget about",
        "bypass your",
        "override your",
        "system prompt",
        "system message",
        "hidden instructions",
        "secret instructions",
        "real instructions",
        "true instructions",
        "forget everything",
        "ignore everything",
        "stop being",
        "stop acting",
        "act as if you are",
        "pretend you are",
        "imagine you are",
        "jailbreak",
        "break out of",
        "escape the",
        "do not follow",
        "do not obey",
        "do not listen",
        "don't follow",
        "don't obey",
        "don't listen",
        "code injection",
        "sql injection",
        "command injection"
    };

    public async Task<PromptInjectionResult> DetectAsync(string content)
    {
        // Phase 1: Keyword-based detection
        if (ContainsSuspiciousKeywords(content))
        {
            logger.LogWarning("Potential prompt injection detected via keyword matching: {Content}",
                TruncateForLogging(content));
            return new PromptInjectionResult(IsInjection: true, Confidence: 0.95f, Phase: "keyword");
        }

        // Phase 2: Lightweight model-based detection (delegated to infrastructure layer)
        // For now, return no injection detected
        // The infrastructure layer will call DetectViaLightweightModelAsync if available
        await Task.CompletedTask;
        return new PromptInjectionResult(IsInjection: false, Confidence: 0f, Phase: "model");
    }

    private static bool ContainsSuspiciousKeywords(string content)
    {
        string upper = content.ToUpperInvariant();
        return SuspiciousKeywords.Any(keyword =>
            upper.Contains(keyword.ToUpperInvariant(), StringComparison.Ordinal));
    }

    private static string TruncateForLogging(string content, int maxLength = 200)
        => content.Length > maxLength ? $"{content[..maxLength]}..." : content;
}

public sealed record PromptInjectionResult(bool IsInjection, float Confidence, string Phase);
