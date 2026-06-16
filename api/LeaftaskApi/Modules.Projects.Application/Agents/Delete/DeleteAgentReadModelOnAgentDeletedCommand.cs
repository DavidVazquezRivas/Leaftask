using BuildingBlocks.Application.Commands;

namespace Modules.Projects.Application.Agents.Delete;

public sealed record DeleteAgentReadModelOnAgentDeletedCommand(Guid AgentId, Guid ProjectId) : ICommand;
