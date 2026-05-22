using BuildingBlocks.Application.Queries;

namespace Modules.Organizations.Application.Management.GetMyOrganizations;

public interface IGetMyOrganizationsQueryService
{
    Task<PaginatedResult<SimpleOrganizationDto>> GetMyOrganizationsAsync(
        Guid userId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default);
}
