using BuildingBlocks.Domain.Events;

namespace Modules.Agents.Domain.Events;

public sealed record AgentDeletedDomainEvent(Guid AgentId, Guid ProjectId) : IDomainEvent;
