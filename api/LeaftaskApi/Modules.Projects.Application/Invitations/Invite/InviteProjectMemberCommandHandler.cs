using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Domain.Entities.Invitation;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Invitations.Invite;

public sealed class InviteProjectMemberCommandHandler(
    IProjectRoleRepository roleRepository,
    IProjectMemberRepository memberRepository,
    IProjectInvitationRepository invitationRepository,
    IUserReadModelRepository userReadModelRepository)
    : ICommandHandler<InviteProjectMemberCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(InviteProjectMemberCommand command, CancellationToken cancellationToken)
    {
        bool userExists = await userReadModelRepository.ExistsByIdAsync(command.UserId, cancellationToken);
        if (!userExists)
        {
            return Result.Failure<Guid>(ProjectErrors.OwnerNotFound);
        }

        bool roleExists = await roleRepository.GetByIdTrackedAsync(command.ProjectId, command.RoleId, cancellationToken) is not null;
        if (!roleExists)
        {
            return Result.Failure<Guid>(ProjectErrors.RoleNotFound);
        }

        bool alreadyMember = await memberRepository.ExistsByMemberIdAsync(command.ProjectId, command.UserId, cancellationToken);
        if (alreadyMember)
        {
            return Result.Failure<Guid>(ProjectErrors.UserAlreadyMember);
        }

        ProjectInvitation? existing = await invitationRepository.GetByInviteeTrackedAsync(
            command.ProjectId, command.UserId, cancellationToken);

        if (existing is not null)
        {
            if (existing.Status == InvitationStatus.Pending)
            {
                return Result.Failure<Guid>(ProjectErrors.UserAlreadyInvited);
            }

            existing.Reactivate(command.RoleId);
            await invitationRepository.SaveChangesAsync(cancellationToken);
            return Result.Success(existing.Id);
        }

        ProjectInvitation invitation = ProjectInvitation.Create(
            command.ProjectId, command.UserId, MemberType.User, command.RoleId);

        await invitationRepository.AddAsync(invitation, cancellationToken);
        await invitationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(invitation.Id);
    }
}
