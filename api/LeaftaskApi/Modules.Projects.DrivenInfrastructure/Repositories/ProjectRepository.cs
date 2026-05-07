using Microsoft.EntityFrameworkCore;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Repositories;

public sealed class ProjectRepository(ProjectsDbContext dbContext) : IProjectRepository
{
    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(project => project.Id == id, cancellationToken);

    public async Task<Project?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Projects
            .FirstOrDefaultAsync(project => project.Id == id, cancellationToken);

    public async Task<bool> ExistsByAbbreviationAsync(
        string abbreviation,
        Guid ownerId,
        Guid? excludeProjectId = null,
        CancellationToken cancellationToken = default) =>
        await dbContext.Projects
            .AsNoTracking()
            .AnyAsync(project => project.Abbreviation == abbreviation
                                 && project.OwnerId == ownerId
                                 && (excludeProjectId == null || project.Id != excludeProjectId),
                cancellationToken);

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default) =>
        await dbContext.Projects.AddAsync(project, cancellationToken);

    public Task RemoveAsync(Project project, CancellationToken cancellationToken = default)
    {
        dbContext.Projects.Remove(project);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
