using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Roles.Create;

namespace Modules.Organizations.Application.Roles.Patch;

public sealed record PatchOrganizationRoleCommand(
    Guid OrganizationId,
    Guid RoleId,
    string? Name,
    IReadOnlyCollection<PatchOrganizationRolePermissionInput>? Permissions)
    : ICommand<Result<OrganizationRoleResponse>>;
