using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Notification.Application.ApprovalRequests.GetMy;
using Modules.Notification.Application.ApprovalRequests.UpdateStatus;
using Modules.Notification.Domain.Entities;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.Domain.Errors;
using Modules.Notification.Domain.Repositories;
using NSubstitute;

namespace Modules.Notification.Application.UnitTests.ApprovalRequests.UpdateStatus;

public class UpdateApprovalStatusCommandHandlerTests
{
    private readonly UpdateApprovalStatusCommandHandler _handler;
    private readonly IApprovalRequestRepository _approvalRepositoryMock;
    private readonly IOrganizationPermissionReadModelRepository _orgPermissionRepositoryMock;
    private readonly IProjectPermissionReadModelRepository _projectPermissionRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;
    private readonly IUserContext _userContextMock;

    public UpdateApprovalStatusCommandHandlerTests()
    {
        _approvalRepositoryMock = Substitute.For<IApprovalRequestRepository>();
        _orgPermissionRepositoryMock = Substitute.For<IOrganizationPermissionReadModelRepository>();
        _projectPermissionRepositoryMock = Substitute.For<IProjectPermissionReadModelRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new UpdateApprovalStatusCommandHandler(
            _approvalRepositoryMock,
            _orgPermissionRepositoryMock,
            _projectPermissionRepositoryMock,
            _userReadModelRepositoryMock,
            _userContextMock);
    }

    private static ApprovalRequest BuildPendingRequest()
    {
        UserReadModel requester = new(Guid.NewGuid(), "Alice", "Smith");
        return ApprovalRequest.Create(ContextType.Organization, Guid.NewGuid(), Guid.NewGuid(), "org.manage", requester);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Approved()
    {
        // Arrange
        Guid resolverId = Guid.NewGuid();
        ApprovalRequest request = BuildPendingRequest();
        UpdateApprovalStatusCommand command = new(request.Id, "approved");

        _userContextMock.UserId.Returns(resolverId);
        _approvalRepositoryMock.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);
        _orgPermissionRepositoryMock.ExistsAsync(resolverId, request.ContextId, request.PermissionName, 2, Arg.Any<CancellationToken>()).Returns(true);
        UserReadModel resolver = new(resolverId, "Bob", "Jones");
        _userReadModelRepositoryMock.GetByIdAsync(resolverId, Arg.Any<CancellationToken>()).Returns(resolver);

        // Act
        Result<ApprovalDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("approved");
        await _approvalRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Rejected()
    {
        // Arrange
        Guid resolverId = Guid.NewGuid();
        ApprovalRequest request = BuildPendingRequest();
        UpdateApprovalStatusCommand command = new(request.Id, "rejected");

        _userContextMock.UserId.Returns(resolverId);
        _approvalRepositoryMock.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);
        _orgPermissionRepositoryMock.ExistsAsync(resolverId, request.ContextId, request.PermissionName, 2, Arg.Any<CancellationToken>()).Returns(true);
        UserReadModel resolver = new(resolverId, "Bob", "Jones");
        _userReadModelRepositoryMock.GetByIdAsync(resolverId, Arg.Any<CancellationToken>()).Returns(resolver);

        // Act
        Result<ApprovalDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("rejected");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NotFound()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        UpdateApprovalStatusCommand command = new(id, "approved");
        _approvalRepositoryMock.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((ApprovalRequest?)null);

        // Act
        Result<ApprovalDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApprovalRequestErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AlreadyResolved()
    {
        // Arrange
        Guid resolverId = Guid.NewGuid();
        ApprovalRequest request = BuildPendingRequest();
        UserReadModel resolver = new(resolverId, "Bob", "Jones");
        request.Approve(resolver);

        UpdateApprovalStatusCommand command = new(request.Id, "approved");
        _userContextMock.UserId.Returns(resolverId);
        _approvalRepositoryMock.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        // Act
        Result<ApprovalDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApprovalRequestErrors.AlreadyResolved);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NoPermission()
    {
        // Arrange
        Guid resolverId = Guid.NewGuid();
        ApprovalRequest request = BuildPendingRequest();
        UpdateApprovalStatusCommand command = new(request.Id, "approved");

        _userContextMock.UserId.Returns(resolverId);
        _approvalRepositoryMock.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);
        _orgPermissionRepositoryMock.ExistsAsync(resolverId, request.ContextId, request.PermissionName, 2, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<ApprovalDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApprovalRequestErrors.Forbidden);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ResolverNotFound()
    {
        // Arrange
        Guid resolverId = Guid.NewGuid();
        ApprovalRequest request = BuildPendingRequest();
        UpdateApprovalStatusCommand command = new(request.Id, "approved");

        _userContextMock.UserId.Returns(resolverId);
        _approvalRepositoryMock.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);
        _orgPermissionRepositoryMock.ExistsAsync(resolverId, request.ContextId, request.PermissionName, 2, Arg.Any<CancellationToken>()).Returns(true);
        _userReadModelRepositoryMock.GetByIdAsync(resolverId, Arg.Any<CancellationToken>()).Returns((UserReadModel?)null);

        // Act
        Result<ApprovalDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApprovalRequestErrors.RequesterNotFound);
    }

    [Fact]
    public async Task Handle_Should_CheckProjectPermission_When_ContextIsProject()
    {
        // Arrange
        Guid resolverId = Guid.NewGuid();
        UserReadModel requester = new(Guid.NewGuid(), "Alice", "Smith");
        ApprovalRequest request = ApprovalRequest.Create(
            ContextType.Project, Guid.NewGuid(), Guid.NewGuid(), "proj.manage", requester);

        UpdateApprovalStatusCommand command = new(request.Id, "approved");
        _userContextMock.UserId.Returns(resolverId);
        _approvalRepositoryMock.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);
        _projectPermissionRepositoryMock.ExistsAsync(resolverId, request.ContextId, request.PermissionName, 2, Arg.Any<CancellationToken>()).Returns(true);
        UserReadModel resolver = new(resolverId, "Bob", "Jones");
        _userReadModelRepositoryMock.GetByIdAsync(resolverId, Arg.Any<CancellationToken>()).Returns(resolver);

        // Act
        Result<ApprovalDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _projectPermissionRepositoryMock.Received(1).ExistsAsync(resolverId, request.ContextId, request.PermissionName, 2, Arg.Any<CancellationToken>());
    }
}
