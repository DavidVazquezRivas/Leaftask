using Modules.Projects.Domain.Entities.Invitation;

namespace Modules.Projects.Domain.Repositories;

public interface IProjectInvitationRepository
{
    Task<List<ProjectInvitation>> GetPendingByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<ProjectInvitation?> GetByIdTrackedAsync(Guid projectId, Guid invitationId, CancellationToken cancellationToken = default);
    Task<ProjectInvitation?> GetByInviteeTrackedAsync(Guid projectId, Guid inviteeId, CancellationToken cancellationToken = default);
    Task AddAsync(ProjectInvitation invitation, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
