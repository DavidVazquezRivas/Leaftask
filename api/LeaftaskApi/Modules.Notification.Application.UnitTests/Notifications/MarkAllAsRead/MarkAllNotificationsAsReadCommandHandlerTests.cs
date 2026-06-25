using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Notification.Application.Notifications.MarkAllAsRead;
using Modules.Notification.Domain.Entities.Notification;
using Modules.Notification.Domain.Repositories;
using NSubstitute;
using NotificationEntity = Modules.Notification.Domain.Entities.Notification.Notification;

namespace Modules.Notification.Application.UnitTests.Notifications.MarkAllAsRead;

public class MarkAllNotificationsAsReadCommandHandlerTests
{
    private readonly MarkAllNotificationsAsReadCommandHandler _handler;
    private readonly INotificationRepository _repositoryMock;
    private readonly IUserContext _userContextMock;

    public MarkAllNotificationsAsReadCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<INotificationRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new MarkAllNotificationsAsReadCommandHandler(_repositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_MarkAllUnreadNotificationsAsRead()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        NotificationEntity n1 = NotificationEntity.Create(NotificationType.Mention, Guid.NewGuid(), Guid.NewGuid(), userId);
        NotificationEntity n2 = NotificationEntity.Create(NotificationType.Assignment, Guid.NewGuid(), Guid.NewGuid(), userId);
        _repositoryMock.GetByRecipientIdAsync(userId, Arg.Any<CancellationToken>()).Returns([n1, n2]);

        // Act
        Result result = await _handler.Handle(new MarkAllNotificationsAsReadCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        n1.ReadAt.Should().NotBeNull();
        n2.ReadAt.Should().NotBeNull();
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_SkipAlreadyReadNotifications()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        NotificationEntity n1 = NotificationEntity.Create(NotificationType.Mention, Guid.NewGuid(), Guid.NewGuid(), userId);
        n1.Read();
        NotificationEntity n2 = NotificationEntity.Create(NotificationType.Assignment, Guid.NewGuid(), Guid.NewGuid(), userId);
        _repositoryMock.GetByRecipientIdAsync(userId, Arg.Any<CancellationToken>()).Returns([n1, n2]);

        // Act
        Result result = await _handler.Handle(new MarkAllNotificationsAsReadCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        n2.ReadAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_NotCallSaveChanges_When_NoUnreadNotifications()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        NotificationEntity n1 = NotificationEntity.Create(NotificationType.Mention, Guid.NewGuid(), Guid.NewGuid(), userId);
        n1.Read();
        _repositoryMock.GetByRecipientIdAsync(userId, Arg.Any<CancellationToken>()).Returns([n1]);

        // Act
        Result result = await _handler.Handle(new MarkAllNotificationsAsReadCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
