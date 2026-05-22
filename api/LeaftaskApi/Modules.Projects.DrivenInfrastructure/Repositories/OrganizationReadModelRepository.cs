using Microsoft.EntityFrameworkCore;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Repositories;

public sealed class OrganizationReadModelRepository(ProjectsDbContext dbContext) : IOrganizationReadModelRepository
{
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.OrganizationReadModels.AsNoTracking()
            .AnyAsync(organization => organization.Id == id, cancellationToken);

    public async Task<OrganizationReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.OrganizationReadModels
            .AsNoTracking()
            .FirstOrDefaultAsync(organization => organization.Id == id, cancellationToken);

    public async Task AddAsync(OrganizationReadModel organizationReadModel,
        CancellationToken cancellationToken = default) =>
        await dbContext.OrganizationReadModels.AddAsync(organizationReadModel, cancellationToken);

    public Task RemoveAsync(OrganizationReadModel organizationReadModel,
        CancellationToken cancellationToken = default)
    {
        dbContext.OrganizationReadModels.Remove(organizationReadModel);
        return Task.CompletedTask;
    }
}
