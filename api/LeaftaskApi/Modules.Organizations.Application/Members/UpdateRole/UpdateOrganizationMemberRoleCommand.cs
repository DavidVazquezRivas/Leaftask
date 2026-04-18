using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Members.UpdateRole;

public sealed record UpdateOrganizationMemberRoleCommand(
    Guid OrganizationId,
    Guid MemberId,
    Guid RoleId)
    : ICommand<Result>;
