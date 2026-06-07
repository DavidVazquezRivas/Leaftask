using BuildingBlocks.Application.Commands;

namespace Modules.Agents.Application.Agents.Resume;

public sealed record TryResumeAgentExecutionsCommand(
    string EventType,
    string CorrelationId,
    string NewMessage) : ICommand;
