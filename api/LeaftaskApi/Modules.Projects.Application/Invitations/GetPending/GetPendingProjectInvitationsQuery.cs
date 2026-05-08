using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Invitations.GetPending;

public sealed record GetPendingProjectInvitationsQuery(Guid ProjectId)
    : IQuery<Result<IReadOnlyList<ProjectInvitationDto>>>;
