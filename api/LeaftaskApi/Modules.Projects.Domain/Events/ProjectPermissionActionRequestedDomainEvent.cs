using BuildingBlocks.Domain.Events;

namespace Modules.Projects.Domain.Events;

public sealed record ProjectPermissionActionRequestedDomainEvent(
    Guid ProjectId,
    Guid RequestedByUserId,
    string PermissionName,
    string ActionName,
    string ActionPayload) : IDomainEvent;
