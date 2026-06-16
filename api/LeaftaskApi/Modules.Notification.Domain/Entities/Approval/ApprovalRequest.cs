using BuildingBlocks.Domain.Entities;
using Modules.Notification.Domain.Events;

namespace Modules.Notification.Domain.Entities.Approval;

public sealed class ApprovalRequest : Entity
{
    private ApprovalRequest() { }

    private ApprovalRequest(Guid id, RequestStatus status, ContextType contextType, Guid contextId, Guid targetId,
        string permissionName, DateTime createdAt, UserReadModel requester, UserReadModel? approverRejecter,
        string? actionType, string? actionPayload)
    {
        Id = id;
        Status = status;
        ContextType = contextType;
        ContextId = contextId;
        TargetId = targetId;
        PermissionName = permissionName;
        CreatedAt = createdAt;
        Requester = requester;
        ApproverRejecter = approverRejecter;
        ActionType = actionType;
        ActionPayload = actionPayload;
    }

    private readonly List<RequestComment> _comments = [];

    public Guid Id { get; }
    public RequestStatus Status { get; set; }
    public ContextType ContextType { get; set; }
    public Guid ContextId { get; set; }
    public Guid TargetId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string? ActionType { get; set; }
    public string? ActionPayload { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserReadModel Requester { get; set; } = null!;
    public UserReadModel? ApproverRejecter { get; set; }
    public IReadOnlyCollection<RequestComment> Comments => _comments.AsReadOnly();

    public static ApprovalRequest Create(ContextType contextType, Guid contextId, Guid targetId,
        string permissionName, UserReadModel requester, string? actionType = null, string? actionPayload = null)
    {
        return new ApprovalRequest(Guid.NewGuid(), RequestStatus.Pending, contextType, contextId, targetId,
            permissionName, DateTime.UtcNow, requester, null, actionType, actionPayload);
    }

    public RequestComment AddComment(UserReadModel author, string content)
    {
        RequestComment comment = RequestComment.Create(Id, content, author);
        _comments.Add(comment);
        return comment;
    }

    public void Approve(UserReadModel approver)
    {
        Status = RequestStatus.Approved;
        ApproverRejecter = approver;
        Raise(new ApprovalRequestResolvedDomainEvent(Id, ContextType, ContextId, TargetId, RequestStatus.Approved, ActionType, ActionPayload));
    }

    public void Reject(UserReadModel rejecter)
    {
        Status = RequestStatus.Rejected;
        ApproverRejecter = rejecter;
        Raise(new ApprovalRequestResolvedDomainEvent(Id, ContextType, ContextId, TargetId, RequestStatus.Rejected, null, null));
    }
}
