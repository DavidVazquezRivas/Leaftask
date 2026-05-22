using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Invitations.UpdateStatus;

public sealed record UpdateProjectInvitationStatusCommand(Guid ProjectId, Guid InvitationId, string Status)
    : ICommand<Result>;
