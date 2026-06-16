namespace Modules.Agents.Application.Bootstrap;

public sealed record BootstrapAgentResult(
    string SystemPrompt,
    Guid ModelId,
    double Temperature,
    int MaxTokens,
    IReadOnlyList<BootstrapTimeTrigger> TimeTriggers,
    IReadOnlyList<BootstrapEventTrigger> EventTriggers);

public sealed record BootstrapTimeTrigger(string Name, string CronExpression, string TimeZone);

public sealed record BootstrapEventTrigger(string EventType, string UserPrompt);
