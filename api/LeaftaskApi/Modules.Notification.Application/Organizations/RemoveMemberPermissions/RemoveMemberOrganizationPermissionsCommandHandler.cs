using BuildingBlocks.Application.Commands;
using Modules.Notification.Domain.Repositories;

namespace Modules.Notification.Application.Organizations.RemoveMemberPermissions;

public sealed class RemoveMemberOrganizationPermissionsCommandHandler(
    IOrganizationPermissionReadModelRepository repository)
    : ICommandHandler<RemoveMemberOrganizationPermissionsCommand>
{
    public async Task Handle(RemoveMemberOrganizationPermissionsCommand request, CancellationToken cancellationToken)
    {
        await repository.DeleteByMemberAsync(request.UserId, request.OrganizationId, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
