using BuildingBlocks.Application.Commands;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Organizations.Delete;

public sealed class DeleteOrganizationReadModelOnOrganizationDeletedCommandHandler(
    IOrganizationReadModelRepository organizationReadModelRepository)
    : ICommandHandler<DeleteOrganizationReadModelOnOrganizationDeletedCommand>
{
    public async Task Handle(DeleteOrganizationReadModelOnOrganizationDeletedCommand request, CancellationToken cancellationToken)
    {
        OrganizationReadModel? organizationReadModel =
            await organizationReadModelRepository.GetByIdAsync(request.OrganizationId, cancellationToken);

        if (organizationReadModel is null)
        {
            return;
        }

        await organizationReadModelRepository.RemoveAsync(organizationReadModel, cancellationToken);
    }
}
