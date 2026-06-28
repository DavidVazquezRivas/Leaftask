using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Notification.Application.ApprovalRequests.AddComment;
using Modules.Notification.Application.ApprovalRequests.GetMy;
using Modules.Notification.Domain.Entities;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.Domain.Errors;
using Modules.Notification.Domain.Repositories;
using NSubstitute;

namespace Modules.Notification.Application.UnitTests.ApprovalRequests.AddComment;

public class AddApprovalCommentCommandHandlerTests
{
    private readonly AddApprovalCommentCommandHandler _handler;
    private readonly IApprovalRequestRepository _approvalRepositoryMock;
    private readonly IOrganizationPermissionReadModelRepository _orgPermissionRepositoryMock;
    private readonly IProjectPermissionReadModelRepository _projectPermissionRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;
    private readonly IUserContext _userContextMock;

    public AddApprovalCommentCommandHandlerTests()
    {
        _approvalRepositoryMock = Substitute.For<IApprovalRequestRepository>();
        _orgPermissionRepositoryMock = Substitute.For<IOrganizationPermissionReadModelRepository>();
        _projectPermissionRepositoryMock = Substitute.For<IProjectPermissionReadModelRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new AddApprovalCommentCommandHandler(
            _approvalRepositoryMock,
            _orgPermissionRepositoryMock,
            _projectPermissionRepositoryMock,
            _userReadModelRepositoryMock,
            _userContextMock);
    }

    private static ApprovalRequest BuildApprovalRequest(Guid requesterId)
    {
        UserReadModel requester = new(requesterId, "Alice", "Smith");
        return ApprovalRequest.Create(ContextType.Organization, Guid.NewGuid(), Guid.NewGuid(), "org.manage", requester);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RequesterAddsComment()
    {
        // Arrange
        Guid requesterId = Guid.NewGuid();
        ApprovalRequest approvalRequest = BuildApprovalRequest(requesterId);
        AddApprovalCommentCommand command = new(approvalRequest.Id, "Looks good to me.");

        _userContextMock.UserId.Returns(requesterId);
        _approvalRepositoryMock.GetByIdAsync(approvalRequest.Id, Arg.Any<CancellationToken>()).Returns(approvalRequest);

        UserReadModel author = new(requesterId, "Alice", "Smith");
        _userReadModelRepositoryMock.GetByIdAsync(requesterId, Arg.Any<CancellationToken>()).Returns(author);

        // Act
        Result<ApprovalCommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Content.Should().Be("Looks good to me.");
        await _approvalRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ApprovalNotFound()
    {
        // Arrange
        Guid approvalId = Guid.NewGuid();
        AddApprovalCommentCommand command = new(approvalId, "Comment");
        _approvalRepositoryMock.GetByIdAsync(approvalId, Arg.Any<CancellationToken>()).Returns((ApprovalRequest?)null);

        // Act
        Result<ApprovalCommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApprovalRequestErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NonRequester_HasNoPermission()
    {
        // Arrange
        Guid requesterId = Guid.NewGuid();
        Guid commenterId = Guid.NewGuid();
        ApprovalRequest approvalRequest = BuildApprovalRequest(requesterId);
        AddApprovalCommentCommand command = new(approvalRequest.Id, "Denied");

        _userContextMock.UserId.Returns(commenterId);
        _approvalRepositoryMock.GetByIdAsync(approvalRequest.Id, Arg.Any<CancellationToken>()).Returns(approvalRequest);
        _orgPermissionRepositoryMock.ExistsAsync(commenterId, approvalRequest.ContextId, approvalRequest.PermissionName, 2, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<ApprovalCommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApprovalRequestErrors.Forbidden);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AuthorNotFound()
    {
        // Arrange
        Guid requesterId = Guid.NewGuid();
        ApprovalRequest approvalRequest = BuildApprovalRequest(requesterId);
        AddApprovalCommentCommand command = new(approvalRequest.Id, "Comment");

        _userContextMock.UserId.Returns(requesterId);
        _approvalRepositoryMock.GetByIdAsync(approvalRequest.Id, Arg.Any<CancellationToken>()).Returns(approvalRequest);
        _userReadModelRepositoryMock.GetByIdAsync(requesterId, Arg.Any<CancellationToken>()).Returns((UserReadModel?)null);

        // Act
        Result<ApprovalCommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApprovalRequestErrors.RequesterNotFound);
    }

    [Fact]
    public async Task Handle_Should_CheckProjectPermission_When_ContextIsProject()
    {
        // Arrange
        Guid requesterId = Guid.NewGuid();
        Guid commenterId = Guid.NewGuid();
        UserReadModel requester = new(requesterId, "Bob", "Jones");
        ApprovalRequest approvalRequest = ApprovalRequest.Create(
            ContextType.Project, Guid.NewGuid(), Guid.NewGuid(), "proj.manage", requester);

        AddApprovalCommentCommand command = new(approvalRequest.Id, "Comment on project");
        _userContextMock.UserId.Returns(commenterId);
        _approvalRepositoryMock.GetByIdAsync(approvalRequest.Id, Arg.Any<CancellationToken>()).Returns(approvalRequest);
        _projectPermissionRepositoryMock.ExistsAsync(commenterId, approvalRequest.ContextId, approvalRequest.PermissionName, 2, Arg.Any<CancellationToken>()).Returns(true);

        UserReadModel author = new(commenterId, "Carol", "White");
        _userReadModelRepositoryMock.GetByIdAsync(commenterId, Arg.Any<CancellationToken>()).Returns(author);

        // Act
        Result<ApprovalCommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _projectPermissionRepositoryMock.Received(1).ExistsAsync(commenterId, approvalRequest.ContextId, approvalRequest.PermissionName, 2, Arg.Any<CancellationToken>());
    }
}
