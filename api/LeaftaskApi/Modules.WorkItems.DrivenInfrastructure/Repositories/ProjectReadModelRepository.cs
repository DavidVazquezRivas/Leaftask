using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Repositories;

public sealed class ProjectReadModelRepository(WorkItemsDbContext dbContext) : IProjectReadModelRepository
{
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectReadModels.AsNoTracking()
            .AnyAsync(p => p.Id == id, cancellationToken);

    public async Task<ProjectReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectReadModels
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task AddAsync(ProjectReadModel projectReadModel, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectReadModels.AddAsync(projectReadModel, cancellationToken);

    public void Remove(ProjectReadModel projectReadModel) =>
        dbContext.ProjectReadModels.Remove(projectReadModel);
}
