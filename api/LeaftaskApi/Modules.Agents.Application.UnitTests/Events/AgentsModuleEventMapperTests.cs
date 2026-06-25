using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using FluentAssertions;
using Modules.Agents.Application.Events;
using Modules.Agents.Domain.Events;
using Modules.Agents.Integration;

namespace Modules.Agents.Application.UnitTests.Events;

public class AgentsModuleEventMapperTests
{
    private sealed record UnknownDomainEvent : IDomainEvent;

    private readonly AgentsModuleEventMapper _mapper = new();

    [Fact]
    public void Map_Should_MapAgentCreatedDomainEvent()
    {
        // Arrange
        AgentCreatedDomainEvent domainEvent = new(Guid.NewGuid(), "My Agent", Guid.NewGuid(), Guid.NewGuid());

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeOfType<AgentCreatedIntegrationEvent>();
        AgentCreatedIntegrationEvent integrationEvent = (AgentCreatedIntegrationEvent)result!;
        integrationEvent.AgentId.Should().Be(domainEvent.AgentId);
        integrationEvent.Name.Should().Be("My Agent");
    }

    [Fact]
    public void Map_Should_MapAgentDeletedDomainEvent()
    {
        // Arrange
        AgentDeletedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid());

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeOfType<AgentDeletedIntegrationEvent>();
        AgentDeletedIntegrationEvent integrationEvent = (AgentDeletedIntegrationEvent)result!;
        integrationEvent.AgentId.Should().Be(domainEvent.AgentId);
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
