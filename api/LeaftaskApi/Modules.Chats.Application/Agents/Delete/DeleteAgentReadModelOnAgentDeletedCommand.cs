using BuildingBlocks.Application.Commands;

namespace Modules.Chats.Application.Agents.Delete;

public sealed record DeleteAgentReadModelOnAgentDeletedCommand(Guid AgentId) : ICommand;
