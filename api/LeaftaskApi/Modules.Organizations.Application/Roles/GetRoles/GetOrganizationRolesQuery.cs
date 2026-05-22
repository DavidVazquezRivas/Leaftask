using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Roles.GetRoles;

public sealed record GetOrganizationRolesQuery(Guid OrganizationId)
    : IQuery<Result<IReadOnlyList<OrganizationRoleDto>>>;
