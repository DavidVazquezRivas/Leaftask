using BuildingBlocks.Domain.Entities;
using Modules.Notification.Domain.Events;

namespace Modules.Notification.Domain.Entities.Approval;

public sealed class ApprovalRequest : Entity
{
    private ApprovalRequest() { }

    private ApprovalRequest(Guid id, RequestStatus status, ContextType contextType, Guid contextId, Guid targetId,
        string permissionName, DateTime createdAt, UserReadModel requester, UserReadModel? approverRejecter)
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
    }

    private readonly List<RequestComment> _comments = [];

    public Guid Id { get; }
    public RequestStatus Status { get; set; }
    public ContextType ContextType { get; set; }
    public Guid ContextId { get; set; }
    public Guid TargetId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserReadModel Requester { get; set; } = null!;
    public UserReadModel? ApproverRejecter { get; set; }
    public IReadOnlyCollection<RequestComment> Comments => _comments.AsReadOnly();

    public static ApprovalRequest Create(ContextType contextType, Guid contextId, Guid targetId,
        string permissionName, UserReadModel requester)
    {
        return new ApprovalRequest(Guid.NewGuid(), RequestStatus.Pending, contextType, contextId, targetId,
            permissionName, DateTime.UtcNow, requester, null);
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
        Raise(new ApprovalRequestResolvedDomainEvent(Id, ContextType, ContextId, TargetId, RequestStatus.Approved));
    }

    public void Reject(UserReadModel rejecter)
    {
        Status = RequestStatus.Rejected;
        ApproverRejecter = rejecter;
        Raise(new ApprovalRequestResolvedDomainEvent(Id, ContextType, ContextId, TargetId, RequestStatus.Rejected));
    }
}
