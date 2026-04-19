using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Roles.Create;

public sealed class CreateOrganizationRoleCommandHandler(
    IOrganizationRepository organizationRepository,
    IOrganizationPermissionRepository organizationPermissionRepository,
    IGetOrganizationRoleDetailsQueryService getOrganizationRoleDetailsQueryService)
    : ICommandHandler<CreateOrganizationRoleCommand, Result<OrganizationRoleResponse>>
{
    public async Task<Result<OrganizationRoleResponse>> Handle(CreateOrganizationRoleCommand request,
        CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure<OrganizationRoleResponse>(OrganizationErrors.OrganizationNotFound);
        }

        IReadOnlyCollection<OrganizationPermission> availablePermissions =
            await organizationPermissionRepository.GetAllAsync(cancellationToken);

        HashSet<Guid> availablePermissionIds = availablePermissions.Select(permission => permission.Id).ToHashSet();
        if (request.Permissions.Any(permission => !availablePermissionIds.Contains(permission.OrganizationPermissionId)))
        {
            return Result.Failure<OrganizationRoleResponse>(OrganizationErrors.OrganizationPermissionNotFound);
        }

        OrganizationRole role = new(request.Name, organization.Id, availablePermissions);

        foreach (CreateOrganizationRolePermissionInput permission in request.Permissions)
        {
            role.SetPermissionLevel(permission.OrganizationPermissionId, permission.Level);
        }

        await organizationRepository.AddRoleAsync(role, cancellationToken);
        await organizationRepository.SaveChangesAsync(cancellationToken);

        OrganizationRoleResponse? response = await getOrganizationRoleDetailsQueryService.GetOrganizationRoleAsync(
            organization.Id,
            role.Id,
            cancellationToken);

        if (response is null)
        {
            return Result.Failure<OrganizationRoleResponse>(OrganizationErrors.OrganizationRoleNotFound);
        }

        return Result.Success(response);
    }
}
