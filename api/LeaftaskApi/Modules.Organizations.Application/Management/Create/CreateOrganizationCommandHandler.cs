using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Management.Create;

public class CreateOrganizationCommandHandler(
    IOrganizationRepository organizationRepository,
    IGetBasicOrganizationQueryService getBasicOrganizationQueryService)
    : ICommandHandler<CreateOrganizationCommand, Result<BasicOrganizationResponse>>
{
    public async Task<Result<BasicOrganizationResponse>> Handle(CreateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        Organization organization = Organization.Create(request.Name, request.Description, request.Website);

        await organizationRepository.AddAsync(organization, cancellationToken);

        await organizationRepository.SaveChangesAsync(cancellationToken);

        BasicOrganizationResponse? response =
            await getBasicOrganizationQueryService.GetBasicOrganizationsAsync(organization.Id, cancellationToken);

        if (response == null)
        {
            return Result.Failure<BasicOrganizationResponse>(OrganizationErrors.OrganizationNotFound);
        }

        return Result.Success(response);
    }
}
