using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Authorization;

namespace Modules.Organizations.Application.Management.Patch;

[RequireOrganizationPermission("Configure Organization")]
public sealed record PatchOrganizationCommand(
    Guid Id,
    string? Name,
    string? Description,
    string? Website)
    : ICommand<Result<BasicOrganizationResponse>>, IOrganizationPermissionRequest
{
    public Guid OrganizationId => Id;
}
