using BuildingBlocks.Application.Commands;

namespace Modules.Projects.Application.Agents.Create;

public sealed record CreateAgentReadModelOnAgentCreatedCommand(
    Guid AgentId,
    string Name,
    Guid ProjectId,
    Guid RoleId) : ICommand;
