using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Domain.Repositories;

public interface IOrganizationRepository
{
    Task AddAsync(Organization organization, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
