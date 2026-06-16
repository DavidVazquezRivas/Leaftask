using BuildingBlocks.Domain.Events;

namespace Modules.Agents.Domain.Events;

public sealed record AgentCreatedDomainEvent(
    Guid AgentId,
    string Name,
    Guid ProjectId,
    Guid RoleId) : IDomainEvent;
