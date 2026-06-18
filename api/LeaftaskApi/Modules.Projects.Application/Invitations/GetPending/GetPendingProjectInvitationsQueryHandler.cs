using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Invitations.GetPending;

public sealed class GetPendingProjectInvitationsQueryHandler(
    IProjectRepository projectRepository,
    IProjectAccessChecker accessChecker,
    IUserContext userContext,
    IGetPendingProjectInvitationsQueryService queryService)
    : IQueryHandler<GetPendingProjectInvitationsQuery, Result<IReadOnlyList<ProjectInvitationDto>>>
{
    public async Task<Result<IReadOnlyList<ProjectInvitationDto>>> Handle(
        GetPendingProjectInvitationsQuery query,
        CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure<IReadOnlyList<ProjectInvitationDto>>(ProjectErrors.ProjectNotFound);

        if (!await accessChecker.CanAccessAsync(project, userContext.UserId, cancellationToken))
            return Result.Failure<IReadOnlyList<ProjectInvitationDto>>(ProjectErrors.AccessDenied);

        IReadOnlyList<ProjectInvitationDto> invitations =
            await queryService.GetPendingAsync(query.ProjectId, cancellationToken);

        return Result.Success(invitations);
    }
}
