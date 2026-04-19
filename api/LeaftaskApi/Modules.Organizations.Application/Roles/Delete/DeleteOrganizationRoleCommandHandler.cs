using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Roles.Delete;

public sealed class DeleteOrganizationRoleCommandHandler(
    IOrganizationRepository organizationRepository)
    : ICommandHandler<DeleteOrganizationRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteOrganizationRoleCommand request, CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure(OrganizationErrors.OrganizationNotFound);
        }

        Result removeResult = organization.RemoveRole(request.RoleId);
        if (removeResult.IsFailure)
        {
            return Result.Failure(removeResult.Error);
        }

        await organizationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
