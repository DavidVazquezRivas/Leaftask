using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Events;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Invitations.Create;

public sealed class CreateOrganizationInvitationCommandHandler(
    IOrganizationRepository organizationRepository)
    : ICommandHandler<CreateOrganizationInvitationCommand, Result<OrganizationInvitationResponse>>
{
    public async Task<Result<OrganizationInvitationResponse>> Handle(CreateOrganizationInvitationCommand request,
        CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.OrganizationNotFound);
        }

        OrganizationRole? role = organization.Roles.FirstOrDefault(role => role.Id == request.RoleId);
        if (role is null)
        {
            return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.OrganizationRoleNotFound);
        }

        bool hasActiveInvitation = organization.Invitations.Any(invitation =>
            invitation.UserId == request.UserId
            && invitation.Status is InvitationStatus.Pending or InvitationStatus.Accepted);

        if (hasActiveInvitation)
        {
            return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.InvalidInvitationStatus);
        }

        OrganizationInvitation invitation = OrganizationInvitation.Create(request.OrganizationId, request.UserId, request.RoleId);
        invitation.Raise(new OrganizationInvitationCreatedDomainEvent(invitation.Id, invitation.OrganizationId, invitation.UserId, invitation.OrganizationRoleId));

        await organizationRepository.AddInvitationAsync(invitation, cancellationToken);
        await organizationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(ToResponse(invitation));
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
