using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Authorization;

namespace Modules.Organizations.Application.Members.UpdateRole;

[RequireOrganizationPermission("Configure Organization")]
public sealed record UpdateOrganizationMemberRoleCommand(
    Guid OrganizationId,
    Guid MemberId,
    Guid RoleId)
    : ICommand<Result>, IOrganizationPermissionRequest;
