using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Notification.Application.Notifications.DeleteByTarget;
using Modules.Notification.Domain.Entities.Notification;
using Modules.Notification.Domain.Repositories;
using NSubstitute;
using NotificationEntity = Modules.Notification.Domain.Entities.Notification.Notification;

namespace Modules.Notification.Application.UnitTests.Notifications.DeleteByTarget;

public class DeleteNotificationByTargetCommandHandlerTests
{
    private readonly DeleteNotificationByTargetCommandHandler _handler;
    private readonly INotificationRepository _repositoryMock;

    public DeleteNotificationByTargetCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<INotificationRepository>();
        _handler = new DeleteNotificationByTargetCommandHandler(_repositoryMock);
    }

    [Fact]
    public async Task Handle_Should_DeleteNotification_When_Found()
    {
        // Arrange
        Guid targetId = Guid.NewGuid();
        NotificationEntity notification = NotificationEntity.Create(NotificationType.Invitation, Guid.NewGuid(), targetId, Guid.NewGuid());
        DeleteNotificationByTargetCommand command = new(targetId);

        _repositoryMock.GetByTargetIdAsync(targetId, Arg.Any<CancellationToken>()).Returns(notification);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Received(1).Delete(notification);
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_NotFound()
    {
        // Arrange
        Guid targetId = Guid.NewGuid();
        DeleteNotificationByTargetCommand command = new(targetId);
        _repositoryMock.GetByTargetIdAsync(targetId, Arg.Any<CancellationToken>()).Returns((NotificationEntity?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _repositoryMock.DidNotReceive().Delete(Arg.Any<NotificationEntity>());
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
