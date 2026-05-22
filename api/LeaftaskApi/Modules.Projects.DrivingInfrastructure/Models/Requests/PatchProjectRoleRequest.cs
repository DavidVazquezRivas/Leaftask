using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.DrivingInfrastructure.Models.Requests;

public sealed record PatchProjectRoleRequest(
    string? Name,
    IReadOnlyList<PatchProjectRolePermissionRequest>? Permissions);

public sealed record PatchProjectRolePermissionRequest(Guid Id, PermissionLevel Level);
