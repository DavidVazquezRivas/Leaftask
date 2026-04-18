using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Management;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Management.Patch;

public sealed class PatchOrganizationCommandHandler(
    IOrganizationRepository organizationRepository,
    IGetOrganizationDetailsQueryService getOrganizationDetailsQueryService)
    : ICommandHandler<PatchOrganizationCommand, Result<BasicOrganizationResponse>>
{
    public async Task<Result<BasicOrganizationResponse>> Handle(PatchOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (organization is null)
        {
            return Result.Failure<BasicOrganizationResponse>(OrganizationErrors.OrganizationNotFound);
        }

        organization.Update(request.Name, request.Description, request.Website);

        await organizationRepository.SaveChangesAsync(cancellationToken);

        BasicOrganizationResponse? response =
            await getOrganizationDetailsQueryService.GetOrganizationDetailsAsync(organization.Id, cancellationToken);

        if (response is null)
        {
            return Result.Failure<BasicOrganizationResponse>(OrganizationErrors.OrganizationNotFound);
        }

        return Result.Success(response);
    }
}
