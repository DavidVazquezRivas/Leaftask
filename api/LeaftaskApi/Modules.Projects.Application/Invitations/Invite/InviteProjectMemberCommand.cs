using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.Projects.Application.Invitations.Invite;

[RequireProjectPermission("project.invite-members")]
public sealed record InviteProjectMemberCommand(Guid ProjectId, Guid UserId, Guid RoleId)
    : ICommand<Result<Guid>>, IProjectPermissionRequest;
