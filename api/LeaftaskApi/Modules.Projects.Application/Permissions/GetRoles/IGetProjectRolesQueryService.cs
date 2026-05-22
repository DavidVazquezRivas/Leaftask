namespace Modules.Projects.Application.Permissions.GetRoles;

public interface IGetProjectRolesQueryService
{
    Task<IReadOnlyList<ProjectRoleDto>> GetRolesAsync(Guid projectId, CancellationToken cancellationToken = default);
}
