using FluentAssertions;
using Modules.Notification.Domain.Entities;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.Domain.Events;

namespace Modules.Notification.Domain.UnitTests.Entities;

public class ApprovalRequestTests
{
    private static UserReadModel BuildUser(Guid? id = null) =>
        new(id ?? Guid.NewGuid(), "Jane", "Doe");

    [Fact]
    public void Create_Should_SetPendingStatus_And_StoreFields()
    {
        // Arrange
        UserReadModel requester = BuildUser();
        Guid contextId = Guid.NewGuid();
        Guid targetId = Guid.NewGuid();

        // Act
        ApprovalRequest request = ApprovalRequest.Create(
            ContextType.Organization, contextId, targetId,
            "org.manage", requester, "DELETE", "{\"id\":\"1\"}");

        // Assert
        request.Status.Should().Be(RequestStatus.Pending);
        request.ContextType.Should().Be(ContextType.Organization);
        request.ContextId.Should().Be(contextId);
        request.TargetId.Should().Be(targetId);
        request.PermissionName.Should().Be("org.manage");
        request.Requester.Should().Be(requester);
        request.ActionType.Should().Be("DELETE");
        request.ActionPayload.Should().Be("{\"id\":\"1\"}");
        request.Comments.Should().BeEmpty();
        request.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Should_Work_WithoutOptionalFields()
    {
        // Arrange
        UserReadModel requester = BuildUser();

        // Act
        ApprovalRequest request = ApprovalRequest.Create(
            ContextType.Project, Guid.NewGuid(), Guid.NewGuid(), "proj.view", requester);

        // Assert
        request.Status.Should().Be(RequestStatus.Pending);
        request.ActionType.Should().BeNull();
        request.ActionPayload.Should().BeNull();
    }

    [Fact]
    public void AddComment_Should_AppendCommentToCollection()
    {
        // Arrange
        UserReadModel requester = BuildUser();
        UserReadModel author = BuildUser();
        ApprovalRequest request = ApprovalRequest.Create(
            ContextType.Organization, Guid.NewGuid(), Guid.NewGuid(), "org.manage", requester);

        // Act
        RequestComment comment = request.AddComment(author, "Please review carefully.");

        // Assert
        request.Comments.Should().ContainSingle();
        comment.Content.Should().Be("Please review carefully.");
        comment.CreatedBy.Should().Be(author);
        comment.RequestId.Should().Be(request.Id);
        comment.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Approve_Should_SetApprovedStatus_And_RaiseDomainEvent()
    {
        // Arrange
        UserReadModel requester = BuildUser();
        UserReadModel approver = BuildUser();
        ApprovalRequest request = ApprovalRequest.Create(
            ContextType.Organization, Guid.NewGuid(), Guid.NewGuid(), "org.manage", requester, "DELETE", "{\"x\":1}");

        // Act
        request.Approve(approver);

        // Assert
        request.Status.Should().Be(RequestStatus.Approved);
        request.ApproverRejecter.Should().Be(approver);
        request.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ApprovalRequestResolvedDomainEvent>()
            .Which.Status.Should().Be(RequestStatus.Approved);
    }

    [Fact]
    public void Reject_Should_SetRejectedStatus_And_RaiseDomainEvent()
    {
        // Arrange
        UserReadModel requester = BuildUser();
        UserReadModel rejecter = BuildUser();
        ApprovalRequest request = ApprovalRequest.Create(
            ContextType.Project, Guid.NewGuid(), Guid.NewGuid(), "proj.manage", requester);

        // Act
        request.Reject(rejecter);

        // Assert
        request.Status.Should().Be(RequestStatus.Rejected);
        request.ApproverRejecter.Should().Be(rejecter);
        request.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ApprovalRequestResolvedDomainEvent>()
            .Which.Status.Should().Be(RequestStatus.Rejected);
    }

    [Fact]
    public void Reject_Should_Clear_ActionPayload_In_Event()
    {
        // Arrange
        UserReadModel requester = BuildUser();
        ApprovalRequest request = ApprovalRequest.Create(
            ContextType.Organization, Guid.NewGuid(), Guid.NewGuid(), "org.manage", requester, "POST", "{\"x\":1}");

        // Act
        request.Reject(BuildUser());

        // Assert
        ApprovalRequestResolvedDomainEvent evt = (ApprovalRequestResolvedDomainEvent)request.DomainEvents.Single();
        evt.ActionPayload.Should().BeNull();
    }
}
