using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Authorization;

namespace Modules.Organizations.Application.Roles.Delete;

[RequireOrganizationPermission("Configure Organization")]
public sealed record DeleteOrganizationRoleCommand(
    Guid OrganizationId,
    Guid RoleId)
    : ICommand<Result>, IOrganizationPermissionRequest;
