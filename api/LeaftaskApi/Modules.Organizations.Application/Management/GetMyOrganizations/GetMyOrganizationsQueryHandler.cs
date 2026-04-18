using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Management.GetMyOrganizations;

public sealed class GetMyOrganizationsQueryHandler(
    IGetMyOrganizationsQueryService service,
    IUserContext userContext)
    : IQueryHandler<GetMyOrganizationsQuery, Result<PaginatedResult<SimpleOrganizationDto>>>
{
    public async Task<Result<PaginatedResult<SimpleOrganizationDto>>> Handle(GetMyOrganizationsQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResult<SimpleOrganizationDto> organizations = await service.GetMyOrganizationsAsync(
            userContext.UserId,
            request.Limit,
            request.Cursor,
            request.Sort,
            cancellationToken);

        return Result.Success(organizations);
    }
}
