using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.DrivingInfrastructure.Models.Requests;

public sealed record CreateProjectRoleRequest(
    string Name,
    IReadOnlyList<CreateProjectRolePermissionRequest> Permissions);

public sealed record CreateProjectRolePermissionRequest(Guid Id, PermissionLevel Level);
