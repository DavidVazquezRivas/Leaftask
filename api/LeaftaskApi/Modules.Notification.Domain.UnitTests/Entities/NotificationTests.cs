using FluentAssertions;
using NotificationEntity = Modules.Notification.Domain.Entities.Notification.Notification;
using Modules.Notification.Domain.Entities.Notification;

namespace Modules.Notification.Domain.UnitTests.Entities;

public class NotificationTests
{
    [Fact]
    public void Create_Should_SetFields_And_ReadAtNull()
    {
        // Arrange
        Guid contextId = Guid.NewGuid();
        Guid targetId = Guid.NewGuid();
        Guid recipientId = Guid.NewGuid();
        Guid actorId = Guid.NewGuid();

        // Act
        NotificationEntity notification = NotificationEntity.Create(
            NotificationType.Mention, contextId, targetId, recipientId, actorId);

        // Assert
        notification.Id.Should().NotBe(Guid.Empty);
        notification.Type.Should().Be(NotificationType.Mention);
        notification.ContextId.Should().Be(contextId);
        notification.TargetId.Should().Be(targetId);
        notification.RecipientId.Should().Be(recipientId);
        notification.ActorId.Should().Be(actorId);
        notification.ReadAt.Should().BeNull();
        notification.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_Should_Work_WithoutActor()
    {
        // Act
        NotificationEntity notification = NotificationEntity.Create(
            NotificationType.Assignment, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Assert
        notification.ActorId.Should().BeNull();
    }

    [Fact]
    public void Read_Should_SetReadAt()
    {
        // Arrange
        NotificationEntity notification = NotificationEntity.Create(
            NotificationType.Invitation, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        notification.Read();

        // Assert
        notification.ReadAt.Should().NotBeNull();
        notification.ReadAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
