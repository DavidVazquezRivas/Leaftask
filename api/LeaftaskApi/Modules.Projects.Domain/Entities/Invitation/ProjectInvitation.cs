using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Events;

namespace Modules.Projects.Domain.Entities.Invitation;

public sealed class ProjectInvitation : Entity
{
    private ProjectInvitation() { }

    private ProjectInvitation(
        Guid id, Guid projectId, Guid inviteeId, MemberType inviteeType, Guid roleId,
        InvitationStatus status, DateTime invitedAt, DateTime? respondedAt, DateTime? cancelledAt)
    {
        Id = id;
        ProjectId = projectId;
        InviteeId = inviteeId;
        InviteeType = inviteeType;
        RoleId = roleId;
        Status = status;
        InvitedAt = invitedAt;
        RespondedAt = respondedAt;
        CancelledAt = cancelledAt;
    }

    public Guid Id { get; }
    public Guid ProjectId { get; }
    public Guid InviteeId { get; }
    public MemberType InviteeType { get; }
    public Guid RoleId { get; private set; }
    public InvitationStatus Status { get; private set; }
    public DateTime InvitedAt { get; }
    public DateTime? RespondedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public static ProjectInvitation Create(Guid projectId, Guid inviteeId, MemberType inviteeType, Guid roleId)
    {
        ProjectInvitation invitation = new(Guid.NewGuid(), projectId, inviteeId, inviteeType, roleId,
            InvitationStatus.Pending, DateTime.UtcNow, null, null);
        invitation.Raise(new ProjectInvitationCreatedDomainEvent(invitation.Id, projectId, inviteeId));
        return invitation;
    }

    public Result Accept() => Respond(InvitationStatus.Accepted);

    public Result Reject() => Respond(InvitationStatus.Rejected);

    public Result Cancel()
    {
        if (Status != InvitationStatus.Pending)
        {
            return Result.Failure(ProjectErrors.InvalidInvitationStatus);
        }

        Status = InvitationStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        Raise(new ProjectInvitationCancelledDomainEvent(Id, ProjectId, InviteeId));
        return Result.Success();
    }

    public Result Reactivate(Guid newRoleId)
    {
        Status = InvitationStatus.Pending;
        RoleId = newRoleId;
        RespondedAt = null;
        CancelledAt = null;
        Raise(new ProjectInvitationCreatedDomainEvent(Id, ProjectId, InviteeId));
        return Result.Success();
    }

    private Result Respond(InvitationStatus status)
    {
        if (Status != InvitationStatus.Pending)
        {
            return Result.Failure(ProjectErrors.InvalidInvitationStatus);
        }

        Status = status;
        RespondedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
