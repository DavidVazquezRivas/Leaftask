using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Roles.GetMyPermissions;

public sealed record GetMyOrganizationPermissionsQuery(Guid OrganizationId)
    : IQuery<Result<IReadOnlyList<MyOrganizationPermissionDto>>>;
