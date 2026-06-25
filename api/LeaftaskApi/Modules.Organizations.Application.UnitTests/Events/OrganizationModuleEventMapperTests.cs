using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using FluentAssertions;
using Modules.Organizations.Application.Events;
using Modules.Organizations.Domain.Entities;
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
        OrganizationCreatedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid());
        IIntegrationEvent? result = _mapper.Map(domainEvent);
        result.Should().BeOfType<OrganizationCreatedIntegrationEvent>();
    }

    [Fact]
    public void Map_Should_MapOrganizationDeletedDomainEvent()
    {
        OrganizationDeletedDomainEvent domainEvent = new(Guid.NewGuid());
        IIntegrationEvent? result = _mapper.Map(domainEvent);
        result.Should().BeOfType<OrganizationDeletedIntegrationEvent>();
    }

    [Fact]
    public void Map_Should_MapOrganizationInvitationCreatedDomainEvent()
    {
        OrganizationInvitationCreatedDomainEvent domainEvent = new(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        IIntegrationEvent? result = _mapper.Map(domainEvent);
        result.Should().BeOfType<OrganizationInvitationCreatedIntegrationEvent>();
    }

    [Theory]
    [InlineData(InvitationStatus.Accepted, "accepted")]
    [InlineData(InvitationStatus.Rejected, "rejected")]
    [InlineData(InvitationStatus.Canceled, "canceled")]
    [InlineData(InvitationStatus.Abandoned, "abandoned")]
    [InlineData(InvitationStatus.Pending, "pending")]
    public void Map_Should_MapOrganizationInvitationRespondedDomainEvent(InvitationStatus status, string expectedStatus)
    {
        OrganizationInvitationRespondedDomainEvent domainEvent = new(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), status);
        IIntegrationEvent? result = _mapper.Map(domainEvent);
        OrganizationInvitationRespondedIntegrationEvent @event = result.Should().BeOfType<OrganizationInvitationRespondedIntegrationEvent>().Subject;
        @event.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public void Map_Should_MapOrganizationPermissionActionRequestedDomainEvent()
    {
        OrganizationPermissionActionRequestedDomainEvent domainEvent = new(
            Guid.NewGuid(), Guid.NewGuid(), "permission", "action", "{}");
        IIntegrationEvent? result = _mapper.Map(domainEvent);
        result.Should().BeOfType<OrganizationPermissionActionRequestedIntegrationEvent>();
    }

    [Fact]
    public void Map_Should_MapOrganizationMemberJoinedDomainEvent()
    {
        IReadOnlyCollection<Domain.Events.OrganizationPermissionEntry> permissions = [new("read", 1)];
        OrganizationMemberJoinedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid(), permissions);
        IIntegrationEvent? result = _mapper.Map(domainEvent);
        result.Should().BeOfType<OrganizationMemberJoinedIntegrationEvent>();
    }

    [Fact]
    public void Map_Should_MapOrganizationMemberRoleChangedDomainEvent()
    {
        IReadOnlyCollection<Domain.Events.OrganizationPermissionEntry> permissions = [new("write", 2)];
        OrganizationMemberRoleChangedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid(), permissions);
        IIntegrationEvent? result = _mapper.Map(domainEvent);
        result.Should().BeOfType<OrganizationMemberRoleChangedIntegrationEvent>();
    }

    [Fact]
    public void Map_Should_MapOrganizationRolePermissionsUpdatedDomainEvent()
    {
        IReadOnlyCollection<Domain.Events.OrganizationPermissionEntry> permissions = [new("write", 2)];
        IReadOnlyCollection<Domain.Events.AffectedMemberPermissions> affected = [new(Guid.NewGuid(), permissions)];
        OrganizationRolePermissionsUpdatedDomainEvent domainEvent = new(Guid.NewGuid(), affected);
        IIntegrationEvent? result = _mapper.Map(domainEvent);
        result.Should().BeOfType<OrganizationRolePermissionsUpdatedIntegrationEvent>();
    }

    [Fact]
    public void Map_Should_MapOrganizationMemberRemovedDomainEvent()
    {
        OrganizationMemberRemovedDomainEvent domainEvent = new(Guid.NewGuid(), Guid.NewGuid());
        IIntegrationEvent? result = _mapper.Map(domainEvent);
        result.Should().BeOfType<OrganizationMemberRemovedIntegrationEvent>();
    }

    [Fact]
    public void Map_Should_ReturnNull_When_DomainEventIsUnknown()
    {
        UnknownDomainEvent domainEvent = new();
        IIntegrationEvent? result = _mapper.Map(domainEvent);
        result.Should().BeNull();
    }
}
