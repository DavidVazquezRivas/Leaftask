using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Authorization;

namespace Modules.Organizations.Application.Management.Delete;

[RequireOrganizationPermission("Configure Organization")]
public sealed record DeleteOrganizationCommand(Guid Id)
    : ICommand<Result>, IOrganizationPermissionRequest
{
    public Guid OrganizationId => Id;
}
