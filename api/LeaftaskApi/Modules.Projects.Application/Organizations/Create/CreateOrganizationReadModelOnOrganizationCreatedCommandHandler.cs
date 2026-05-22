using BuildingBlocks.Application.Commands;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Organizations.Create;

public sealed class CreateOrganizationReadModelOnOrganizationCreatedCommandHandler(
    IOrganizationReadModelRepository organizationReadModelRepository)
    : ICommandHandler<CreateOrganizationReadModelOnOrganizationCreatedCommand>
{
    public async Task Handle(CreateOrganizationReadModelOnOrganizationCreatedCommand request, CancellationToken cancellationToken)
    {
        bool exists = await organizationReadModelRepository.ExistsByIdAsync(request.OrganizationId, cancellationToken);
        if (exists)
        {
            return;
        }

        OrganizationReadModel organizationReadModel = new(request.OrganizationId);
        await organizationReadModelRepository.AddAsync(organizationReadModel, cancellationToken);
    }
}
