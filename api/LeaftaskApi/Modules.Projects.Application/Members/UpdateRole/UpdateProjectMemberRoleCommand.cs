using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.Projects.Application.Members.UpdateRole;

[RequireProjectPermission("project.member-roles")]
public sealed record UpdateProjectMemberRoleCommand(Guid ProjectId, Guid MemberId, Guid RoleId)
    : ICommand<Result>, IProjectPermissionRequest;
