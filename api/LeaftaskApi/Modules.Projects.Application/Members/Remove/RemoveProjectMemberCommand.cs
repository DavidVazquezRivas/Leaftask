using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;

namespace Modules.Projects.Application.Members.Remove;

[RequireProjectPermission("project.remove-members")]
public sealed record RemoveProjectMemberCommand(Guid ProjectId, Guid MemberId)
    : ICommand<Result>, IProjectPermissionRequest;
