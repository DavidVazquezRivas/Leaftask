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

        // TODO separate reactivation and creation into different commands
        OrganizationInvitation? existingInvitation = organization.Invitations.FirstOrDefault(inv => inv.UserId == request.UserId);
        if (existingInvitation is not null)
        {
            if (existingInvitation.Status is InvitationStatus.Pending or InvitationStatus.Accepted)
            {
                return Result.Failure<OrganizationInvitationResponse>(OrganizationErrors.InvalidInvitationStatus);
            }

            existingInvitation.Reactivate(request.RoleId);
            await organizationRepository.SaveChangesAsync(cancellationToken);
            return Result.Success(ToResponse(existingInvitation));
        }

        OrganizationInvitation invitation = OrganizationInvitation.Create(request.OrganizationId, request.UserId, request.RoleId);

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
