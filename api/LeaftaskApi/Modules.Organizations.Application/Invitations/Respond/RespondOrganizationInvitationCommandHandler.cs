using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Invitations.Respond;

public sealed class RespondOrganizationInvitationCommandHandler(
    IOrganizationRepository organizationRepository,
    IOrganizationPermissionRepository organizationPermissionRepository,
    IUserContext userContext)
    : ICommandHandler<RespondOrganizationInvitationCommand, Result<OrganizationInvitationResponse>>
{
    private const string InviteMembersPermissionName = "Invite Members";

    public async Task<Result<OrganizationInvitationResponse>> Handle(
        RespondOrganizationInvitationCommand request,
        CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.OrganizationNotFound);
        }

        OrganizationInvitation? invitation = organization.Invitations.FirstOrDefault(invitation => invitation.Id == request.InvitationId);
        if (invitation is null)
        {
            return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.OrganizationInvitationNotFound);
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.InvalidInvitationStatus);
        }

        InvitationStatus desiredStatus = ParseStatus(request.Status);
        if (desiredStatus is InvitationStatus.Accepted or InvitationStatus.Rejected)
        {
            if (invitation.UserId != userContext.UserId)
            {
                return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.OrganizationPermissionDenied);
            }
        }
        else if (desiredStatus == InvitationStatus.Canceled)
        {
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
        }

        Result result = desiredStatus switch
        {
            InvitationStatus.Accepted => invitation.Accept(),
            InvitationStatus.Rejected => invitation.Reject(),
            InvitationStatus.Canceled => invitation.Cancel(),
            _ => Result.Failure(OrganizationErrors.InvalidInvitationStatus)
        };

        if (result.IsFailure)
        {
            return Result.Failure<OrganizationInvitationResponse>(result.Error);
        }

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

    private static InvitationStatus ParseStatus(string status) =>
        status.Trim().ToUpperInvariant() switch
        {
            "ACCEPTED" => InvitationStatus.Accepted,
            "REJECTED" => InvitationStatus.Rejected,
            "CANCELED" => InvitationStatus.Canceled,
            _ => throw new InvalidOperationException($"Unsupported invitation status '{status}'.")
        };

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
