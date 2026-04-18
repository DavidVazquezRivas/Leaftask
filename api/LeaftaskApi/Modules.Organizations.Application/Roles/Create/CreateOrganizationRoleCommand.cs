using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Roles.Create;

public sealed record CreateOrganizationRoleCommand(
    Guid OrganizationId,
    string Name,
    IReadOnlyCollection<CreateOrganizationRolePermissionInput> Permissions)
    : ICommand<Result<OrganizationRoleResponse>>;
