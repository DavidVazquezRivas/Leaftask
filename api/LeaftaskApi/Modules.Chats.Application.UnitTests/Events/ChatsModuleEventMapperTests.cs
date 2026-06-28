using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using FluentAssertions;
using Modules.Chats.Application.Events;
using Modules.Chats.Domain.Events;
using Modules.Chats.Integration;

namespace Modules.Chats.Application.UnitTests.Events;

public class ChatsModuleEventMapperTests
{
    private sealed record UnknownDomainEvent : IDomainEvent;

    private readonly ChatsModuleEventMapper _mapper = new();

    [Fact]
    public void Map_Should_MapMessageCreatedDomainEvent()
    {
        // Arrange
        List<Guid> agentIds = [Guid.NewGuid()];
        MessageCreatedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Hello", agentIds, false);

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeOfType<ChatMessageSentIntegrationEvent>();
        ChatMessageSentIntegrationEvent integrationEvent = (ChatMessageSentIntegrationEvent)result!;
        integrationEvent.ChatId.Should().Be(domainEvent.ChatId);
        integrationEvent.Content.Should().Be("Hello");
    }

    [Fact]
    public void Map_Should_ReturnNull_When_DomainEventIsUnknown()
    {
        // Arrange
        UnknownDomainEvent domainEvent = new();

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeNull();
    }
}
