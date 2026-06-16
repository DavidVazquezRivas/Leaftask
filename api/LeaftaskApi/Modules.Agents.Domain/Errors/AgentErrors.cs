using BuildingBlocks.Domain.Result;

namespace Modules.Agents.Domain.Errors;

public static class AgentErrors
{
    public static readonly Error AgentNotFound = new("Agent.NotFound", "Agent not found", 404);
    public static readonly Error ProjectNotFound = new("Agent.ProjectNotFound", "Project not found", 404);
    public static readonly Error ModelNotFound = new("Agent.ModelNotFound", "Model not found", 404);
    public static readonly Error ProviderNotFound = new("Agent.ProviderNotFound", "Model provider not found", 404);
    public static readonly Error AccessDenied = new("Agent.AccessDenied", "Access denied to this project", 403);
    public static readonly Error BootstrapFailed = new("Agent.BootstrapFailed", "Failed to generate agent configuration from instructions", 422);
}
