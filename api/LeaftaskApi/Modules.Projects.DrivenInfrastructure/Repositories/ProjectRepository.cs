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

    public async Task<bool> ExistsByAbbreviationAsync(string abbreviation, CancellationToken cancellationToken = default) =>
        await dbContext.Projects
            .AsNoTracking()
            .AnyAsync(project => project.Abbreviation == abbreviation, cancellationToken);

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default) =>
        await dbContext.Projects.AddAsync(project, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
