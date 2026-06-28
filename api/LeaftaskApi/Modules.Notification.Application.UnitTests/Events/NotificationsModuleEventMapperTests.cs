using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using FluentAssertions;
using Modules.Notification.Application.Events;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.Domain.Events;
using Modules.Notifications.Integration;

namespace Modules.Notification.Application.UnitTests.Events;

public class NotificationsModuleEventMapperTests
{
    private sealed record UnknownDomainEvent : IDomainEvent;

    private readonly NotificationsModuleEventMapper _mapper = new();

    [Fact]
    public void Map_Should_MapApprovalRequestResolvedDomainEvent()
    {
        // Arrange
        ApprovalRequestResolvedDomainEvent domainEvent = new(
            Guid.NewGuid(), ContextType.Organization, Guid.NewGuid(),
            Guid.NewGuid(), RequestStatus.Approved, "DELETE", "{\"id\":1}");

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeOfType<ApprovalRequestResolvedIntegrationEvent>();
        ApprovalRequestResolvedIntegrationEvent integrationEvent = (ApprovalRequestResolvedIntegrationEvent)result!;
        integrationEvent.RequestId.Should().Be(domainEvent.RequestId);
        integrationEvent.Status.Should().Be("Approved");
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
