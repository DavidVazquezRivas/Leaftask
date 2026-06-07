using BuildingBlocks.Integration;

namespace Modules.Agents.Integration;

public sealed record AgentDeletedIntegrationEvent(Guid AgentId, Guid ProjectId) : IIntegrationEvent;
