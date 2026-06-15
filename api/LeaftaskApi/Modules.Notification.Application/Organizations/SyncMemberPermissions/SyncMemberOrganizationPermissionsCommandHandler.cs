using BuildingBlocks.Application.Commands;
using Modules.Notification.Domain.Entities.Approval.Permission;
using Modules.Notification.Domain.Repositories;

namespace Modules.Notification.Application.Organizations.SyncMemberPermissions;

public sealed class SyncMemberOrganizationPermissionsCommandHandler(
    IOrganizationPermissionReadModelRepository repository)
    : ICommandHandler<SyncMemberOrganizationPermissionsCommand>
{
    public async Task Handle(SyncMemberOrganizationPermissionsCommand request, CancellationToken cancellationToken)
    {
        await repository.DeleteByMemberAsync(request.UserId, request.OrganizationId, cancellationToken);

        IEnumerable<OrganizationPermissionReadModel> newEntries = request.Permissions
            .Select(p => new OrganizationPermissionReadModel(
                Guid.NewGuid(),
                request.UserId,
                request.OrganizationId,
                p.PermissionName,
                p.Level));

        await repository.AddRangeAsync(newEntries, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
