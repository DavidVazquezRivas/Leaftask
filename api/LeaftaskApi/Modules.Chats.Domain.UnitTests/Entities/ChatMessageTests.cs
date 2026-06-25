using FluentAssertions;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Events;

namespace Modules.Chats.Domain.UnitTests.Entities;

public class ChatMessageTests
{
    private static Chat BuildChat() => Chat.Create(Guid.NewGuid(), DateTime.UtcNow);

    private static ChatParticipant BuildParticipant(Chat chat) =>
        new(Guid.NewGuid(), Guid.NewGuid(), ParticipantType.User, DateTime.UtcNow, chat);

    [Fact]
    public void Create_Should_SetFields_And_RaiseMessageCreatedDomainEvent()
    {
        // Arrange
        Chat chat = BuildChat();
        ChatParticipant sender = BuildParticipant(chat);
        Guid messageId = Guid.NewGuid();
        List<Guid> agentIds = [Guid.NewGuid()];

        // Act
        ChatMessage message = ChatMessage.Create(messageId, "Hello world!", DateTime.UtcNow, MessageStatus.Pending,
            chat, sender, agentIds);

        // Assert
        message.Id.Should().Be(messageId);
        message.Content.Should().Be("Hello world!");
        message.Status.Should().Be(MessageStatus.Pending);
        message.Chat.Should().Be(chat);
        message.Sender.Should().Be(sender);
        message.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<MessageCreatedDomainEvent>()
            .Which.ChatId.Should().Be(chat.Id);
    }

    [Fact]
    public void MarkAsDelivered_Should_UpdateStatus()
    {
        // Arrange
        Chat chat = BuildChat();
        ChatParticipant sender = BuildParticipant(chat);
        ChatMessage message = ChatMessage.Create(Guid.NewGuid(), "Hello", DateTime.UtcNow, MessageStatus.Pending, chat, sender, []);

        // Act
        message.MarkAsDelivered();

        // Assert
        message.Status.Should().Be(MessageStatus.Delivered);
    }

    [Fact]
    public void MarkAsRead_Should_UpdateStatus()
    {
        // Arrange
        Chat chat = BuildChat();
        ChatParticipant sender = BuildParticipant(chat);
        ChatMessage message = ChatMessage.Create(Guid.NewGuid(), "Hello", DateTime.UtcNow, MessageStatus.Pending, chat, sender, []);

        // Act
        message.MarkAsRead();

        // Assert
        message.Status.Should().Be(MessageStatus.Read);
    }

    [Fact]
    public void Create_WithAgentSender_Should_SetSenderIsAgent_True()
    {
        // Arrange
        Chat chat = BuildChat();
        ChatParticipant agentSender = new(Guid.NewGuid(), Guid.NewGuid(), ParticipantType.Agent, DateTime.UtcNow, chat);

        // Act
        ChatMessage message = ChatMessage.Create(Guid.NewGuid(), "I am agent", DateTime.UtcNow, MessageStatus.Pending, chat, agentSender, []);

        // Assert
        MessageCreatedDomainEvent evt = (MessageCreatedDomainEvent)message.DomainEvents.Single();
        evt.SenderIsAgent.Should().BeTrue();
    }
}
