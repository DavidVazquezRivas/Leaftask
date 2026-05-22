using Modules.Projects.Domain.Entities.Owner;

namespace Modules.Projects.Domain.Repositories;

public interface IOrganizationReadModelRepository
{
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrganizationReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(OrganizationReadModel organizationReadModel, CancellationToken cancellationToken = default);
    Task RemoveAsync(OrganizationReadModel organizationReadModel, CancellationToken cancellationToken = default);
}
