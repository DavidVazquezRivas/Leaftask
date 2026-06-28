using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Notification.Application.ApprovalRequests.Create;
using Modules.Notification.Domain.Entities;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.Domain.Errors;
using Modules.Notification.Domain.Repositories;
using NSubstitute;

namespace Modules.Notification.Application.UnitTests.ApprovalRequests.Create;

public class CreateApprovalRequestCommandHandlerTests
{
    private readonly CreateApprovalRequestCommandHandler _handler;
    private readonly IApprovalRequestRepository _approvalRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;

    public CreateApprovalRequestCommandHandlerTests()
    {
        _approvalRepositoryMock = Substitute.For<IApprovalRequestRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _handler = new CreateApprovalRequestCommandHandler(_approvalRepositoryMock, _userReadModelRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithId_When_RequesterExists()
    {
        // Arrange
        Guid requesterId = Guid.NewGuid();
        UserReadModel requester = new(requesterId, "Alice", "Smith");
        CreateApprovalRequestCommand command = new(ContextType.Organization, Guid.NewGuid(), requesterId, "org.manage");

        _userReadModelRepositoryMock.GetByIdAsync(requesterId, Arg.Any<CancellationToken>()).Returns(requester);

        ApprovalRequest? added = null;
        await _approvalRepositoryMock.AddAsync(Arg.Do<ApprovalRequest>(r => added = r), Arg.Any<CancellationToken>());

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        added.Should().NotBeNull();
        added!.PermissionName.Should().Be("org.manage");
        await _approvalRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RequesterNotFound()
    {
        // Arrange
        Guid requesterId = Guid.NewGuid();
        CreateApprovalRequestCommand command = new(ContextType.Organization, Guid.NewGuid(), requesterId, "org.manage");
        _userReadModelRepositoryMock.GetByIdAsync(requesterId, Arg.Any<CancellationToken>()).Returns((UserReadModel?)null);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ApprovalRequestErrors.RequesterNotFound);
        await _approvalRepositoryMock.DidNotReceive().AddAsync(Arg.Any<ApprovalRequest>(), Arg.Any<CancellationToken>());
    }
}
