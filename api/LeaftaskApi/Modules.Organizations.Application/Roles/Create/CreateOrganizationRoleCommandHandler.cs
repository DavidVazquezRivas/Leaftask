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
    IGetOrganizationRoleDetailsQueryService getOrganizationRoleDetailsQueryService,
    IUserContext userContext)
    : ICommandHandler<CreateOrganizationRoleCommand, Result<OrganizationRoleResponse>>
{
    private const string ConfigureOrganizationPermissionName = "Configure Organization";

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

        OrganizationPermission? configureOrganizationPermission = availablePermissions.FirstOrDefault(permission =>
            permission.Name.Equals(ConfigureOrganizationPermissionName, StringComparison.OrdinalIgnoreCase));

        if (configureOrganizationPermission is null)
        {
            return Result.Failure<OrganizationRoleResponse>(OrganizationErrors.OrganizationPermissionNotFound);
        }

        if (!HasConfigureOrganizationPermission(organization, configureOrganizationPermission.Id))
        {
            return Result.Failure<OrganizationRoleResponse>(OrganizationErrors.OrganizationPermissionDenied);
        }

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

    private bool HasConfigureOrganizationPermission(Organization organization, Guid configureOrganizationPermissionId)
    {
        OrganizationInvitation? invitation = organization.Invitations.FirstOrDefault(inv =>
            inv.UserId == userContext.UserId && inv.Status == InvitationStatus.Accepted);

        if (invitation is null)
        {
            return false;
        }

        OrganizationRole? role = organization.Roles.FirstOrDefault(role => role.Id == invitation.OrganizationRoleId);
        if (role is null)
        {
            return false;
        }

        OrganizationRolePermission? rolePermission = role.Permissions.FirstOrDefault(permission =>
            permission.OrganizationPermissionId == configureOrganizationPermissionId);

        return rolePermission is not null && rolePermission.Level == PermissionLevel.Full;
    }
}
