namespace Modules.Projects.Application.Invitations.GetPending;

public interface IGetPendingProjectInvitationsQueryService
{
    Task<IReadOnlyList<ProjectInvitationDto>> GetPendingAsync(Guid projectId, CancellationToken cancellationToken = default);
}
