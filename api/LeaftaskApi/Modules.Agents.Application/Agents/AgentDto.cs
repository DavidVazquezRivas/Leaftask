namespace Modules.Agents.Application.Agents;

public sealed record AgentDto(
    Guid Id,
    Guid ProjectId,
    string Name,
    string Instructions,
    Guid? TemplateId,
    DateTime CreatedAt);
