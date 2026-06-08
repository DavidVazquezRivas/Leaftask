using BuildingBlocks.Application.Commands;

namespace Modules.WorkItems.Application.Agents.Create;

public sealed record CreateUserReadModelOnAgentCreatedCommand(Guid AgentId, string Name) : ICommand;
