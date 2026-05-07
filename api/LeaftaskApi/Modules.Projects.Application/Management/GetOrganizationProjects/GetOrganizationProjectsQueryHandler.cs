using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Management.GetMyProjects;

namespace Modules.Projects.Application.Management.GetOrganizationProjects;

public sealed class GetOrganizationProjectsQueryHandler(
    IGetOrganizationProjectQueryService service)
    : IQueryHandler<GetOrganizationProjectsQuery, Result<PaginatedResult<SimpleProjectDto>>>
{
    public async Task<Result<PaginatedResult<SimpleProjectDto>>> Handle(
        GetOrganizationProjectsQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResult<SimpleProjectDto> projects = await service.GetOrganizationProjectsAsync(
            request.OrganizationId,
            request.Limit,
            request.Cursor,
            request.Sort,
            cancellationToken);
        return Result.Success(projects);
    }
}
