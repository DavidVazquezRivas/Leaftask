using Modules.Projects.Domain.Entities;

namespace Modules.Projects.Application.Authorization;

public interface IProjectAccessChecker
{
    Task<bool> CanAccessAsync(Project project, Guid userId, CancellationToken cancellationToken = default);
}
