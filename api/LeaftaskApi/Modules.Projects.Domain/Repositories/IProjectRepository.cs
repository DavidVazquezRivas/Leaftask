using Modules.Projects.Domain.Entities;

namespace Modules.Projects.Domain.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByAbbreviationAsync(string abbreviation, Guid ownerId, Guid? excludeProjectId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    Task RemoveAsync(Project project, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
