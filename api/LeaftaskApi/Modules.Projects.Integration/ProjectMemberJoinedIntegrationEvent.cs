using BuildingBlocks.Integration;

namespace Modules.Projects.Integration;

public sealed record ProjectMemberJoinedIntegrationEvent(
    Guid ProjectId,
    Guid UserId,
    IReadOnlyCollection<ProjectPermissionEntryDto> Permissions) : IIntegrationEvent;

public sealed record ProjectPermissionEntryDto(string PermissionName, int Level);
