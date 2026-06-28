using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using FluentAssertions;
using Modules.WorkItems.Application.Events;
using Modules.WorkItems.Domain.Events;
using Modules.WorkItems.Integration;

namespace Modules.WorkItems.Application.UnitTests.Events;

public class WorkItemsModuleEventMapperTests
{
    private sealed record UnknownDomainEvent : IDomainEvent;

    private readonly WorkItemsModuleEventMapper _mapper = new();

    [Fact]
    public void Map_Should_MapWorkItemCreatedDomainEvent()
    {
        // Arrange
        WorkItemCreatedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid(), "My task", Guid.NewGuid(), Guid.NewGuid(), null);

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeOfType<WorkItemCreatedIntegrationEvent>();
        ((WorkItemCreatedIntegrationEvent)result!).Title.Should().Be("My task");
    }

    [Fact]
    public void Map_Should_MapWorkItemDeletedDomainEvent()
    {
        // Arrange
        WorkItemDeletedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid());

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeOfType<WorkItemDeletedIntegrationEvent>();
    }

    [Fact]
    public void Map_Should_MapWorkItemStatusChangedDomainEvent()
    {
        // Arrange
        WorkItemStatusChangedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeOfType<WorkItemStatusChangedIntegrationEvent>();
    }

    [Fact]
    public void Map_Should_MapCommentAddedDomainEvent_WithMentions()
    {
        // Arrange
        List<Guid> mentions = [Guid.NewGuid()];
        CommentAddedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), mentions);

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeOfType<UsersMentionedInCommentIntegrationEvent>();
    }

    [Fact]
    public void Map_Should_ReturnNull_When_CommentAddedWithNoMentions()
    {
        // Arrange
        CommentAddedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), []);

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeNull();
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
