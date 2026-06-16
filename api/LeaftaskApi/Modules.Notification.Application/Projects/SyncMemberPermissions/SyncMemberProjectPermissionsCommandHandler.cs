using BuildingBlocks.Application.Commands;
using Modules.Notification.Domain.Entities.Approval.Permission;
using Modules.Notification.Domain.Repositories;

namespace Modules.Notification.Application.Projects.SyncMemberPermissions;

public sealed class SyncMemberProjectPermissionsCommandHandler(
    IProjectPermissionReadModelRepository repository)
    : ICommandHandler<SyncMemberProjectPermissionsCommand>
{
    public async Task Handle(SyncMemberProjectPermissionsCommand request, CancellationToken cancellationToken)
    {
        await repository.DeleteByMemberAsync(request.UserId, request.ProjectId, cancellationToken);

        IEnumerable<ProjectPermissionReadModel> newEntries = request.Permissions
            .Select(p => new ProjectPermissionReadModel(
                Guid.NewGuid(),
                request.UserId,
                request.ProjectId,
                p.PermissionName,
                p.Level));

        await repository.AddRangeAsync(newEntries, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
