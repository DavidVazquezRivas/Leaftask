using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Invitation;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Invitations.UpdateStatus;

public sealed class UpdateProjectInvitationStatusCommandHandler(
    IProjectRepository projectRepository,
    IProjectRoleRepository roleRepository,
    IProjectMemberRepository memberRepository,
    IProjectInvitationRepository invitationRepository,
    IProjectPermissionChecker permissionChecker,
    IUserContext userContext)
    : ICommandHandler<UpdateProjectInvitationStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateProjectInvitationStatusCommand command, CancellationToken cancellationToken)
    {
        ProjectInvitation? invitation = await invitationRepository.GetByIdTrackedAsync(
            command.ProjectId, command.InvitationId, cancellationToken);

        if (invitation is null)
        {
            return Result.Failure(ProjectErrors.InvitationNotFound);
        }

        string status = command.Status.ToUpperInvariant();

        if (status == "CANCELLED")
        {
            return await HandleCancelAsync(invitation, command.ProjectId, cancellationToken);
        }

        return await HandleRespondAsync(invitation, status, command.ProjectId, cancellationToken);
    }

    private async Task<Result> HandleRespondAsync(
        ProjectInvitation invitation,
        string status,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        if (invitation.InviteeId != userContext.UserId)
        {
            return Result.Failure(ProjectErrors.InvitationAccessDenied);
        }

        Result actionResult = status == "ACCEPTED"
            ? invitation.Accept()
            : invitation.Reject();

        if (actionResult.IsFailure)
        {
            return actionResult;
        }

        if (status == "ACCEPTED")
        {
            Result memberResult = await AddMemberAsync(projectId, invitation, cancellationToken);
            if (memberResult.IsFailure)
            {
                return memberResult;
            }
        }

        await invitationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> HandleCancelAsync(
        ProjectInvitation invitation,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        ProjectPermissionCheckStatus checkStatus = await permissionChecker.CheckAsync(
            projectId, userContext.UserId, "project.invite-members", cancellationToken);

        if (checkStatus != ProjectPermissionCheckStatus.Full)
        {
            return Result.Failure(ProjectErrors.AccessDenied);
        }

        Result cancelResult = invitation.Cancel();
        if (cancelResult.IsFailure)
        {
            return cancelResult;
        }

        await invitationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> AddMemberAsync(
        Guid projectId,
        ProjectInvitation invitation,
        CancellationToken cancellationToken)
    {
        Project? project = await projectRepository.GetByIdTrackedAsync(projectId, cancellationToken);
        if (project is null)
        {
            return Result.Failure(ProjectErrors.ProjectNotFound);
        }

        ProjectRole? role = await roleRepository.GetByIdTrackedAsync(projectId, invitation.RoleId, cancellationToken);
        if (role is null)
        {
            return Result.Failure(ProjectErrors.RoleNotFound);
        }

        ProjectMember member = new(Guid.NewGuid(), invitation.InviteeId, MemberType.User, role, project);
        await memberRepository.AddAsync(member, cancellationToken);

        return Result.Success();
    }
}
