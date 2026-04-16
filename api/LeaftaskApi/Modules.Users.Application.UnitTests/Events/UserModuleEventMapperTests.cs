using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using FluentAssertions;
using Modules.Users.Application.Events;
using Modules.Users.Domain.Events;
using Modules.Users.Domain.UnitTests.TestBuilders;
using Modules.Users.Integration;

namespace Modules.Users.Application.UnitTests.Events;

#pragma warning disable CA1515
public sealed record UnknownDomainEvent : IDomainEvent;
#pragma warning restore CA1515

public class UserModuleEventMapperTests
{
    private readonly UserModuleEventMapper _mapper = new();

    [Fact]
    public void Map_Should_ReturnUserCreatedIntegrationEvent_When_DomainEventIsUserCreated()
    {
        // Arrange
        UserCreatedDomainEvent domainEvent = UserCreatedDomainEventTestBuilder.AnEvent()
            .WithFirstName("Clark")
            .WithEmail("clark@dailyplanet.com")
            .Build();

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<UserCreatedIntegrationEvent>();

        UserCreatedIntegrationEvent? integrationEvent = result as UserCreatedIntegrationEvent;
        integrationEvent!.UserId.Should().Be(domainEvent.UserId);
        integrationEvent.FirstName.Should().Be(domainEvent.FirstName);
        integrationEvent.LastName.Should().Be(domainEvent.LastName);
        integrationEvent.Email.Should().Be(domainEvent.Email);
    }

    [Fact]
    public void Map_Should_ReturnNull_When_DomainEventIsUnknown()
    {
        // Arrange
        UnknownDomainEvent unknownEvent = new();

        // Act
        IIntegrationEvent? result = _mapper.Map(unknownEvent);

        // Assert
        result.Should().BeNull();
    }
}
