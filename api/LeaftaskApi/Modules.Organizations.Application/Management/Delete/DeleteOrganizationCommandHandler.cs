using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Management.Delete;

public sealed class DeleteOrganizationCommandHandler(IOrganizationRepository organizationRepository)
    : ICommandHandler<DeleteOrganizationCommand, Result>
{
    public async Task<Result> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (organization is null)
        {
            return Result.Failure(OrganizationErrors.OrganizationNotFound);
        }

        await organizationRepository.RemoveAsync(organization, cancellationToken);
        await organizationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
