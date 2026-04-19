using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using FluentAssertions;
using Modules.Organizations.Application.Events;
using Modules.Organizations.Domain.Events;
using Modules.Organizations.Integration;

namespace Modules.Organizations.Application.UnitTests.Events;

public class OrganizationModuleEventMapperTests
{
    private sealed record UnknownDomainEvent : IDomainEvent;

    private readonly OrganizationModuleEventMapper _mapper = new();

    [Fact]
    public void Map_Should_MapOrganizationCreatedDomainEvent()
    {
        // Arrange
        OrganizationCreatedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid());

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeOfType<OrganizationCreatedIntegrationEvent>();
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
