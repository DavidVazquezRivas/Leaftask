using BuildingBlocks.Application.Commands;

namespace Modules.WorkItems.Application.Agents.Delete;

public sealed record DeleteUserReadModelOnAgentDeletedCommand(Guid AgentId) : ICommand;
