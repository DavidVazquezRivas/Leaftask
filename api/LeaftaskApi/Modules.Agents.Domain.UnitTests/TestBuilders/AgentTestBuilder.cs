using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Entities.Model;

namespace Modules.Agents.Domain.UnitTests.TestBuilders;

#pragma warning disable CA1515
public sealed class AgentTestBuilder
#pragma warning restore CA1515
{
    private Guid _id = Guid.NewGuid();
    private Guid _projectId = Guid.NewGuid();
    private string _name = "Test Agent";
    private string _instructions = "Monitor overdue tasks daily";
    private string _systemPrompt = "You are a helpful project management assistant";
    private Guid? _templateId;
    private DateTime _createdAt = DateTime.UtcNow;
    private readonly List<(string EventType, string UserPrompt)> _eventTriggers = [];
    private readonly List<(string Name, string CronExpression, string TimeZone)> _timeTriggers = [];

    private AgentTestBuilder() { }

    public static AgentTestBuilder AnAgent() => new();

    public AgentTestBuilder WithId(Guid id) { _id = id; return this; }
    public AgentTestBuilder WithProjectId(Guid projectId) { _projectId = projectId; return this; }
    public AgentTestBuilder WithName(string name) { _name = name; return this; }
    public AgentTestBuilder WithInstructions(string instructions) { _instructions = instructions; return this; }
    public AgentTestBuilder WithSystemPrompt(string systemPrompt) { _systemPrompt = systemPrompt; return this; }
    public AgentTestBuilder WithTemplateId(Guid templateId) { _templateId = templateId; return this; }
    public AgentTestBuilder WithCreatedAt(DateTime createdAt) { _createdAt = createdAt; return this; }

    public AgentTestBuilder WithEventTrigger(string eventType, string userPrompt = "Handle this event")
    {
        _eventTriggers.Add((eventType, userPrompt));
        return this;
    }

    public AgentTestBuilder WithTimeTrigger(string name = "Daily", string cronExpression = "0 9 * * *", string timeZone = "UTC")
    {
        _timeTriggers.Add((name, cronExpression, timeZone));
        return this;
    }

    public Agent Build()
    {
        ModelProvider provider = new(Guid.NewGuid(), "OpenAI", string.Empty);
        Model model = new(Guid.NewGuid(), "gpt-test", provider, "Test model", 0.2);
        ModelConfig modelConfig = new(Guid.NewGuid(), model, 0.7, 1024);

        return Agent.Create(
            _id,
            _projectId,
            _name,
            _instructions,
            _systemPrompt,
            modelConfig,
            _templateId,
            _createdAt,
            _eventTriggers,
            _timeTriggers);
    }
}
