using BuildingBlocks.Application.Queries;
using Modules.Projects.Application.Management.GetMyProjects;

namespace Modules.Projects.Application.Management.GetOrganizationProjects;

public interface IGetOrganizationProjectQueryService
{
    Task<PaginatedResult<SimpleProjectDto>> GetOrganizationProjectsAsync(
        Guid organizationId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default);
}
