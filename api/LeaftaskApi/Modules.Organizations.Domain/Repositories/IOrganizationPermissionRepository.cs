using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Domain.Repositories;

public interface IOrganizationPermissionRepository
{
    Task<IReadOnlyCollection<OrganizationPermission>> GetAllAsync(CancellationToken cancellationToken = default);
}
