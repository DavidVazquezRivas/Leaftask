using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Management;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Management.Create;

public sealed class CreateOrganizationCommandHandler(
    IOrganizationRepository organizationRepository,
    IOrganizationPermissionRepository organizationPermissionRepository,
    IGetOrganizationDetailsQueryService getOrganizationDetailsQueryService,
    IUserContext userContext)
    : ICommandHandler<CreateOrganizationCommand, Result<BasicOrganizationResponse>>
{
    public async Task<Result<BasicOrganizationResponse>> Handle(CreateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<OrganizationPermission> permissions =
            await organizationPermissionRepository.GetAllAsync(cancellationToken);

        Organization organization = Organization.Create(
            request.Name,
            request.Description,
            request.Website,
            userContext.UserId,
            permissions);

        await organizationRepository.AddAsync(organization, cancellationToken);

        await organizationRepository.SaveChangesAsync(cancellationToken);

        BasicOrganizationResponse? response =
            await getOrganizationDetailsQueryService.GetOrganizationDetailsAsync(organization.Id, cancellationToken);

        if (response == null)
        {
            return Result.Failure<BasicOrganizationResponse>(OrganizationErrors.OrganizationNotFound);
        }

        return Result.Success(response);
    }
}
