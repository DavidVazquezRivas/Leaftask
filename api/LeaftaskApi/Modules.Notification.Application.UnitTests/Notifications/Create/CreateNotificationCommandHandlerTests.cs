using FluentAssertions;
using Modules.Notification.Application.Notifications.Create;
using Modules.Notification.Domain.Entities.Notification;
using Modules.Notification.Domain.Repositories;
using NSubstitute;
using NotificationEntity = Modules.Notification.Domain.Entities.Notification.Notification;

namespace Modules.Notification.Application.UnitTests.Notifications.Create;

public class CreateNotificationCommandHandlerTests
{
    private readonly CreateNotificationCommandHandler _handler;
    private readonly INotificationRepository _repositoryMock;

    public CreateNotificationCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<INotificationRepository>();
        _handler = new CreateNotificationCommandHandler(_repositoryMock);
    }

    [Fact]
    public async Task Handle_Should_CreateAndPersistNotification()
    {
        // Arrange
        Guid contextId = Guid.NewGuid();
        Guid targetId = Guid.NewGuid();
        Guid recipientId = Guid.NewGuid();
        Guid actorId = Guid.NewGuid();
        CreateNotificationCommand command = new(NotificationType.Invitation, contextId, targetId, recipientId, actorId);

        NotificationEntity? added = null;
        await _repositoryMock.AddAsync(Arg.Do<NotificationEntity>(n => added = n), Arg.Any<CancellationToken>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        added.Should().NotBeNull();
        added!.Type.Should().Be(NotificationType.Invitation);
        added.ContextId.Should().Be(contextId);
        added.TargetId.Should().Be(targetId);
        added.RecipientId.Should().Be(recipientId);
        added.ActorId.Should().Be(actorId);
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
