using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.Domain.Repositories;

public interface IProjectRoleRepository
{
    Task<ProjectRole?> GetByIdTrackedAsync(Guid projectId, Guid roleId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(Guid projectId, string name, Guid? excludeRoleId = null, CancellationToken cancellationToken = default);
    Task<ProjectPermission?> GetPermissionByIdAsync(Guid permissionId, CancellationToken cancellationToken = default);
    Task<List<ProjectRolePermission>> GetRolePermissionsTrackedAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<List<ProjectPermission>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ProjectRole role, CancellationToken cancellationToken = default);
    void Remove(ProjectRole role);
    void RemovePermissions(IEnumerable<ProjectRolePermission> permissions);
    Task AddPermissionsAsync(IEnumerable<ProjectRolePermission> permissions, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
