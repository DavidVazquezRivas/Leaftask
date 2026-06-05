using Modules.Agents.Application.Agents.Create;

namespace Modules.Agents.Application.UnitTests.TestBuilders;

internal sealed class CreateAgentCommandTestBuilder
{
    private Guid _projectId = Guid.NewGuid();
    private string _name = "Test Agent";
    private string _instructions = "Monitor overdue tasks daily at 9am";
    private Guid? _templateId;

    private CreateAgentCommandTestBuilder() { }

    public static CreateAgentCommandTestBuilder ACommand() => new();

    public CreateAgentCommandTestBuilder WithProjectId(Guid projectId) { _projectId = projectId; return this; }
    public CreateAgentCommandTestBuilder WithName(string name) { _name = name; return this; }
    public CreateAgentCommandTestBuilder WithInstructions(string instructions) { _instructions = instructions; return this; }
    public CreateAgentCommandTestBuilder WithTemplateId(Guid templateId) { _templateId = templateId; return this; }

    public CreateAgentCommand Build() => new(_projectId, _name, _instructions, _templateId);
}
