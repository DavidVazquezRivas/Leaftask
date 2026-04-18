using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Domain.Repositories;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Organization organization, CancellationToken cancellationToken = default);
    Task AddRoleAsync(OrganizationRole role, CancellationToken cancellationToken = default);
    Task RemoveAsync(Organization organization, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
