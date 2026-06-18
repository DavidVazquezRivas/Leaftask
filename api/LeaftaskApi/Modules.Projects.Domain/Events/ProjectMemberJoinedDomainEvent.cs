using BuildingBlocks.Domain.Events;

namespace Modules.Projects.Domain.Events;

public sealed record ProjectMemberJoinedDomainEvent(
    Guid ProjectId,
    Guid UserId,
    IReadOnlyCollection<ProjectPermissionEntry> Permissions) : IDomainEvent;

public sealed record ProjectPermissionEntry(string PermissionName, int Level);
