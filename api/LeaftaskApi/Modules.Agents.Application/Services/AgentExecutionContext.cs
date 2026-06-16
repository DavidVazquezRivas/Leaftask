namespace Modules.Agents.Application.Services;

public sealed class AgentExecutionContext
{
    public bool IsActive { get; private set; }
    public Guid AgentId { get; private set; }
    public Guid ProjectId { get; private set; }

    public void Activate(Guid agentId, Guid projectId)
    {
        IsActive = true;
        AgentId = agentId;
        ProjectId = projectId;
    }
}
