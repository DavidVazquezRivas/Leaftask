using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Events;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Invitations.Create;

public sealed class CreateOrganizationInvitationCommandHandler(
    IOrganizationRepository organizationRepository,
    IOrganizationPermissionRepository organizationPermissionRepository,
    IUserContext userContext)
    : ICommandHandler<CreateOrganizationInvitationCommand, Result<OrganizationInvitationResponse>>
{
    private const string InviteMembersPermissionName = "Invite Members";

    public async Task<Result<OrganizationInvitationResponse>> Handle(CreateOrganizationInvitationCommand request,
        CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.OrganizationNotFound);
        }

        IReadOnlyCollection<OrganizationPermission> availablePermissions =
            await organizationPermissionRepository.GetAllAsync(cancellationToken);

        OrganizationPermission? inviteMembersPermission = availablePermissions.FirstOrDefault(permission =>
            permission.Name.Equals(InviteMembersPermissionName, StringComparison.OrdinalIgnoreCase));

        if (inviteMembersPermission is null)
        {
            return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.OrganizationPermissionNotFound);
        }

        if (!HasInviteMembersPermission(organization, inviteMembersPermission.Id))
        {
            return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.OrganizationPermissionDenied);
        }

        OrganizationRole? role = organization.Roles.FirstOrDefault(role => role.Id == request.RoleId);
        if (role is null)
        {
            return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.OrganizationRoleNotFound);
        }

        if (organization.Invitations.Any(invitation => invitation.UserId == request.UserId && invitation.Status != InvitationStatus.Abandoned))
        {
            return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.InvalidInvitationStatus);
        }

        OrganizationInvitation invitation = OrganizationInvitation.Create(request.OrganizationId, request.UserId, request.RoleId);
        invitation.Raise(new OrganizationInvitationCreatedDomainEvent(invitation.Id, invitation.OrganizationId, invitation.UserId, invitation.OrganizationRoleId));

        await organizationRepository.AddInvitationAsync(invitation, cancellationToken);
        await organizationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(ToResponse(invitation));
    }

    private bool HasInviteMembersPermission(Organization organization, Guid inviteMembersPermissionId)
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
            permission.OrganizationPermissionId == inviteMembersPermissionId);

        return rolePermission is not null && rolePermission.Level == PermissionLevel.Full;
    }

    private static OrganizationInvitationResponse ToResponse(OrganizationInvitation invitation) =>
        new(
            invitation.Id,
            invitation.OrganizationId,
            invitation.UserId,
            invitation.OrganizationRoleId,
            invitation.Status.ToString(),
            invitation.InvitedAt,
            invitation.RespondedAt,
            invitation.AbandonedAt);
}
