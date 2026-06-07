using BuildingBlocks.Integration;

namespace Modules.Agents.Integration;

public sealed record AgentCreatedIntegrationEvent(
    Guid AgentId,
    string Name,
    Guid ProjectId,
    Guid RoleId) : IIntegrationEvent;
