using BuildingBlocks.Application.Commands;

namespace Modules.Agents.Application.Agents.DirectQuery;

public sealed record HandleDirectAgentQueryCommand(
    Guid AgentId,
    Guid ChatId,
    string Message) : ICommand;
