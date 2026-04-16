using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Events;
using FluentAssertions;
using Xunit;

namespace BuildingBlocks.Domain.UnitTests.Entities;

internal sealed class TestEntity : Entity
{
}

internal record TestDomainEvent : IDomainEvent;

public class EntityTests
{
    [Fact]
    public void Raise_Should_AddDomainEventToCollection()
    {
        // Arrange
        TestEntity entity = new();
        TestDomainEvent domainEvent = new();

        // Act
        entity.Raise(domainEvent);

        // Assert
        entity.DomainEvents.Should().ContainSingle()
            .Which.Should().BeSameAs(domainEvent);
    }

    [Fact]
    public void ClearDomainEvents_Should_EmptyTheCollection()
    {
        // Arrange
        TestEntity entity = new();
        entity.Raise(new TestDomainEvent());
        entity.Raise(new TestDomainEvent());

        // Pre-Assert
        entity.DomainEvents.Should().HaveCount(2);

        // Act
        entity.ClearDomainEvents();

        // Assert
        entity.DomainEvents.Should().BeEmpty();
    }
}
