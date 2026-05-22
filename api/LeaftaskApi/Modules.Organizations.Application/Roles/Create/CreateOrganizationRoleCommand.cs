using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Authorization;

namespace Modules.Organizations.Application.Roles.Create;

[RequireOrganizationPermission("Configure Organization")]
public sealed record CreateOrganizationRoleCommand(
    Guid OrganizationId,
    string Name,
    IReadOnlyCollection<CreateOrganizationRolePermissionInput> Permissions)
    : ICommand<Result<OrganizationRoleResponse>>, IOrganizationPermissionRequest;
