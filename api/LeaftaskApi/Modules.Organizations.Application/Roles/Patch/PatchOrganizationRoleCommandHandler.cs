using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Roles.Create;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Events;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Roles.Patch;

public sealed class PatchOrganizationRoleCommandHandler(
    IOrganizationRepository organizationRepository,
    IOrganizationPermissionRepository organizationPermissionRepository,
    IGetOrganizationRoleDetailsQueryService getOrganizationRoleDetailsQueryService)
    : ICommandHandler<PatchOrganizationRoleCommand, Result<OrganizationRoleResponse>>
{
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

        if (request.Permissions is not null)
        {
            OrganizationRole updatedRole = organization.Roles.Single(r => r.Id == request.RoleId);
            IReadOnlyCollection<OrganizationPermissionEntry> newPermissions = updatedRole.Permissions
                .Select(rp => new OrganizationPermissionEntry(
                    availablePermissions.First(p => p.Id == rp.OrganizationPermissionId).Name,
                    (int)rp.Level))
                .ToArray();

            List<AffectedMemberPermissions> affectedMembers = organization.Invitations
                .Where(inv => inv.OrganizationRoleId == request.RoleId && inv.Status == InvitationStatus.Accepted)
                .Select(inv => new AffectedMemberPermissions(inv.UserId, newPermissions))
                .ToList();

            if (affectedMembers.Count > 0)
            {
                organization.Raise(new OrganizationRolePermissionsUpdatedDomainEvent(organization.Id, affectedMembers));
            }
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
}
