using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Notification.Application.Notifications.MarkAsRead;
using Modules.Notification.Domain.Entities.Notification;
using Modules.Notification.Domain.Errors;
using Modules.Notification.Domain.Repositories;
using NSubstitute;
using NotificationEntity = Modules.Notification.Domain.Entities.Notification.Notification;

namespace Modules.Notification.Application.UnitTests.Notifications.MarkAsRead;

public class MarkNotificationAsReadCommandHandlerTests
{
    private readonly MarkNotificationAsReadCommandHandler _handler;
    private readonly INotificationRepository _repositoryMock;
    private readonly IUserContext _userContextMock;

    public MarkNotificationAsReadCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<INotificationRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new MarkNotificationAsReadCommandHandler(_repositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_MarkNotificationRead()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        NotificationEntity notification = NotificationEntity.Create(NotificationType.Mention, Guid.NewGuid(), Guid.NewGuid(), userId);
        MarkNotificationAsReadCommand command = new(notification.Id);

        _userContextMock.UserId.Returns(userId);
        _repositoryMock.GetByIdAsync(notification.Id, Arg.Any<CancellationToken>()).Returns(notification);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        notification.ReadAt.Should().NotBeNull();
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NotFound()
    {
        // Arrange
        Guid notificationId = Guid.NewGuid();
        MarkNotificationAsReadCommand command = new(notificationId);
        _repositoryMock.GetByIdAsync(notificationId, Arg.Any<CancellationToken>()).Returns((NotificationEntity?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(NotificationErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Forbidden()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();
        NotificationEntity notification = NotificationEntity.Create(NotificationType.Mention, Guid.NewGuid(), Guid.NewGuid(), userId);
        MarkNotificationAsReadCommand command = new(notification.Id);

        _userContextMock.UserId.Returns(otherUserId);
        _repositoryMock.GetByIdAsync(notification.Id, Arg.Any<CancellationToken>()).Returns(notification);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(NotificationErrors.Forbidden);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AlreadyRead()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        NotificationEntity notification = NotificationEntity.Create(NotificationType.Mention, Guid.NewGuid(), Guid.NewGuid(), userId);
        notification.Read();
        MarkNotificationAsReadCommand command = new(notification.Id);

        _userContextMock.UserId.Returns(userId);
        _repositoryMock.GetByIdAsync(notification.Id, Arg.Any<CancellationToken>()).Returns(notification);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(NotificationErrors.AlreadyRead);
    }
}
