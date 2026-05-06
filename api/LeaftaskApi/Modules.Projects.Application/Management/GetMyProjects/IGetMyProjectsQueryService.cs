using BuildingBlocks.Application.Queries;

namespace Modules.Projects.Application.Management.GetMyProjects;

public interface IGetMyProjectsQueryService
{
    Task<PaginatedResult<SimpleProjectDto>> GetMyProjectsAsync(
        Guid userId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default);
}
