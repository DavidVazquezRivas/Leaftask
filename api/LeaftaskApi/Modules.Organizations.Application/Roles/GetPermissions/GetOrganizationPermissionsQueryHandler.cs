using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Roles.GetPermissions;

public sealed class GetOrganizationPermissionsQueryHandler(IGetOrganizationPermissionsQueryService service)
    : IQueryHandler<GetOrganizationPermissionsQuery, Result<IReadOnlyList<OrganizationPermissionDto>>>
{
    public async Task<Result<IReadOnlyList<OrganizationPermissionDto>>> Handle(
        GetOrganizationPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<OrganizationPermissionDto> permissions = await service.GetPermissionsAsync(cancellationToken);
        return Result.Success(permissions);
    }
}
