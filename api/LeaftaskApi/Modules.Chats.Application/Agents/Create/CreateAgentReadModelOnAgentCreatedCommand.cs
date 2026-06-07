using BuildingBlocks.Application.Commands;

namespace Modules.Chats.Application.Agents.Create;

public sealed record CreateAgentReadModelOnAgentCreatedCommand(
    Guid AgentId,
    string Name) : ICommand;
