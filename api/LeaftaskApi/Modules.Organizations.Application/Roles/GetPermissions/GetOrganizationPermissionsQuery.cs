using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Roles.GetPermissions;

public sealed record GetOrganizationPermissionsQuery()
    : IQuery<Result<IReadOnlyList<OrganizationPermissionDto>>>;
