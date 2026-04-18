using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Roles.Create;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Roles.Patch;

public sealed class PatchOrganizationRoleCommandHandler(
    IOrganizationRepository organizationRepository,
    IOrganizationPermissionRepository organizationPermissionRepository,
    IGetOrganizationRoleDetailsQueryService getOrganizationRoleDetailsQueryService,
    IUserContext userContext)
    : ICommandHandler<PatchOrganizationRoleCommand, Result<OrganizationRoleResponse>>
{
    private const string ConfigureOrganizationPermissionName = "Configure Organization";

    public async Task<Result<OrganizationRoleResponse>> Handle(
        PatchOrganizationRoleCommand request,
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

        OrganizationRole? role = organization.Roles.FirstOrDefault(role => role.Id == request.RoleId);
        if (role is null)
        {
            return Result.Failure<OrganizationRoleResponse>(OrganizationErrors.OrganizationRoleNotFound);
        }

        HashSet<Guid> availablePermissionIds = availablePermissions.Select(permission => permission.Id).ToHashSet();
        if (request.Permissions is not null && request.Permissions.Any(permission => !availablePermissionIds.Contains(permission.OrganizationPermissionId)))
        {
            return Result.Failure<OrganizationRoleResponse>(OrganizationErrors.OrganizationPermissionNotFound);
        }

        string name = request.Name ?? role.Name;
        IReadOnlyCollection<(Guid OrganizationPermissionId, PermissionLevel Level)>? permissions = request.Permissions?
            .Select(permission => (permission.OrganizationPermissionId, permission.Level))
            .ToArray();

        Result updateResult = organization.UpdateRole(request.RoleId, name, permissions);
        if (updateResult.IsFailure)
        {
            return Result.Failure<OrganizationRoleResponse>(updateResult.Error);
        }

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
