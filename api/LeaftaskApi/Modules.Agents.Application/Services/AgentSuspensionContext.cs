namespace Modules.Agents.Application.Services;

public sealed class AgentSuspensionContext
{
    public bool ShouldSuspend { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public IReadOnlyList<string> CorrelationIds { get; private set; } = [];

    public void RequestSuspension(string eventType, IReadOnlyList<string> correlationIds)
    {
        ShouldSuspend = true;
        EventType = eventType;
        CorrelationIds = correlationIds;
    }
}
