using FluentAssertions;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Events;

namespace Modules.Chats.Domain.UnitTests.Entities;

public class ChatTests
{
    [Fact]
    public void Create_Should_SetIdAndCreatedAt_And_RaiseChatCreatedDomainEvent()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        DateTime createdAt = DateTime.UtcNow;

        // Act
        Chat chat = Chat.Create(id, createdAt);

        // Assert
        chat.Id.Should().Be(id);
        chat.CreatedAt.Should().Be(createdAt);
        chat.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ChatCreatedDomainEvent>()
            .Which.ChatId.Should().Be(id);
    }
}
