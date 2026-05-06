using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Management.GetMyProjects;

public sealed class GetMyProjectsQueryHandler(
    IGetMyProjectsQueryService service,
    IUserContext userContext)
    : IQueryHandler<GetMyProjectsQuery, Result<PaginatedResult<SimpleProjectDto>>>
{
    public async Task<Result<PaginatedResult<SimpleProjectDto>>> Handle(
        GetMyProjectsQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResult<SimpleProjectDto> projects = await service.GetMyProjectsAsync(
            userContext.UserId,
            request.Limit,
            request.Cursor,
            request.Sort,
            cancellationToken);

        return Result.Success(projects);
    }
}
