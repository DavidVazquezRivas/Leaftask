using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Roles.Delete;

public sealed record DeleteOrganizationRoleCommand(
    Guid OrganizationId,
    Guid RoleId)
    : ICommand<Result>;
