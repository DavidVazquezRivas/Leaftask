using Microsoft.EntityFrameworkCore;
using Modules.Projects.Domain.Entities.Invitation;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Repositories;

public sealed class ProjectInvitationRepository(ProjectsDbContext dbContext) : IProjectInvitationRepository
{
    public async Task<List<ProjectInvitation>> GetPendingByProjectAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectInvitations
            .AsNoTracking()
            .Where(i => i.ProjectId == projectId && i.Status == InvitationStatus.Pending)
            .ToListAsync(cancellationToken);

    public async Task<ProjectInvitation?> GetByIdTrackedAsync(Guid projectId, Guid invitationId, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectInvitations
            .FirstOrDefaultAsync(i => i.ProjectId == projectId && i.Id == invitationId, cancellationToken);

    public async Task<ProjectInvitation?> GetByInviteeTrackedAsync(Guid projectId, Guid inviteeId, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectInvitations
            .FirstOrDefaultAsync(i => i.ProjectId == projectId && i.InviteeId == inviteeId, cancellationToken);

    public async Task AddAsync(ProjectInvitation invitation, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectInvitations.AddAsync(invitation, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
