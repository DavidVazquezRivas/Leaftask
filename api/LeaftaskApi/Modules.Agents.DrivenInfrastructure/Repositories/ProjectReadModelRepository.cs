using Microsoft.EntityFrameworkCore;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.DrivenInfrastructure.Persistence;

namespace Modules.Agents.DrivenInfrastructure.Repositories;

public sealed class ProjectReadModelRepository(AgentsDbContext dbContext) : IProjectReadModelRepository
{
    public async Task<ProjectReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectReadModels
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task AddAsync(ProjectReadModel model, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectReadModels.AddAsync(model, cancellationToken);

    public void Remove(ProjectReadModel model) =>
        dbContext.ProjectReadModels.Remove(model);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
