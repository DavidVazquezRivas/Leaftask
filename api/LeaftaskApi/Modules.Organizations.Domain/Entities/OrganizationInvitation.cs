using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Errors;

namespace Modules.Organizations.Domain.Entities;

public sealed class OrganizationInvitation : Entity
{
    private OrganizationInvitation() { }

    private OrganizationInvitation(Guid id, InvitationStatus status, DateTime invitedAt, DateTime? respondedAt,
        DateTime? abandonedAt,
        Guid organizationId, Guid userId, Guid organizationRoleId)
    {
        Id = id;
        Status = status;
        InvitedAt = invitedAt;
        RespondedAt = respondedAt;
        AbandonedAt = abandonedAt;
        OrganizationId = organizationId;
        UserId = userId;
        OrganizationRoleId = organizationRoleId;
    }

    public Guid Id { get; }
    public InvitationStatus Status { get; private set; }
    public DateTime InvitedAt { get; }
    public DateTime? RespondedAt { get; private set; }
    public DateTime? AbandonedAt { get; private set; }
    public Guid OrganizationId { get; }
    public Guid UserId { get; }
    public Guid OrganizationRoleId { get; }

    public static OrganizationInvitation Create(Guid organizationId, Guid userId, Guid organizationRoleId) =>
        new(Guid.NewGuid(), InvitationStatus.Pending, DateTime.UtcNow, null, null, organizationId, userId, organizationRoleId);

    public Result Accept()
    {
        if (Status != InvitationStatus.Pending)
        {
            return Result.Failure(OrganizationErrors.InvalidInvitationStatus);
        }

        Status = InvitationStatus.Accepted;
        RespondedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Reject()
    {
        if (Status != InvitationStatus.Pending)
        {
            return Result.Failure(OrganizationErrors.InvalidInvitationStatus);
        }

        Status = InvitationStatus.Rejected;
        RespondedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status != InvitationStatus.Pending)
        {
            return Result.Failure(OrganizationErrors.InvalidInvitationStatus);
        }

        Status = InvitationStatus.Canceled;
        RespondedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Abandon()
    {
        if (Status != InvitationStatus.Accepted)
        {
            return Result.Failure(OrganizationErrors.InvalidInvitationStatus);
        }

        Status = InvitationStatus.Abandoned;
        AbandonedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
