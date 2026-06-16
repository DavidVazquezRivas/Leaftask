using BuildingBlocks.Application.Commands;

namespace Modules.Agents.Application.Agents.EnqueueForEvent;

public sealed record EnqueueAgentsForEventTriggerCommand(
    string EventType,
    string Payload) : ICommand;
