using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using FluentAssertions;
using Modules.Organizations.Application.Events;

namespace Modules.Organizations.Application.UnitTests.Events;

public class OrganizationModuleEventMapperTests
{
    private sealed record UnknownDomainEvent : IDomainEvent;

    private readonly OrganizationModuleEventMapper _mapper = new();

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
