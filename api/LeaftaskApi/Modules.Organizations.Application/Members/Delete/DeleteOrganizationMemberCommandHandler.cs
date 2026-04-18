using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Members.Delete;

public sealed class DeleteOrganizationMemberCommandHandler(
    IOrganizationRepository organizationRepository,
    IOrganizationPermissionRepository organizationPermissionRepository,
    IUserContext userContext)
    : ICommandHandler<DeleteOrganizationMemberCommand, Result>
{
    private const string ConfigureOrganizationPermissionName = "Configure Organization";

    public async Task<Result> Handle(DeleteOrganizationMemberCommand request, CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure(OrganizationErrors.OrganizationNotFound);
        }

        IReadOnlyCollection<OrganizationPermission> availablePermissions =
            await organizationPermissionRepository.GetAllAsync(cancellationToken);

        OrganizationPermission? configureOrganizationPermission = availablePermissions.FirstOrDefault(permission =>
            permission.Name.Equals(ConfigureOrganizationPermissionName, StringComparison.OrdinalIgnoreCase));

        if (configureOrganizationPermission is null)
        {
            return Result.Failure(OrganizationErrors.OrganizationPermissionNotFound);
        }

        if (!HasConfigureOrganizationPermission(organization, configureOrganizationPermission.Id))
        {
            return Result.Failure(OrganizationErrors.OrganizationPermissionDenied);
        }

        Result removeResult = organization.RemoveMember(request.MemberId);
        if (removeResult.IsFailure)
        {
            return Result.Failure(removeResult.Error);
        }

        await organizationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
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
