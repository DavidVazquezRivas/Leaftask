using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Repositories;

public class OrganizationRepository(OrganizationDbContext dbContext) : IOrganizationRepository
{
    public async Task AddAsync(Organization organization, CancellationToken cancellationToken = default) =>
        await dbContext.AddAsync(organization, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
