using BuildingBlocks.Integration;
using FluentAssertions;
using Modules.Projects.Application.Events;
using Modules.Projects.Domain.Events;
using Modules.Projects.Integration;

namespace Modules.Projects.Application.UnitTests.Events;

public class ProjectModuleEventMapperTests
{
    private readonly ProjectModuleEventMapper _mapper;

    public ProjectModuleEventMapperTests()
    {
        _mapper = new ProjectModuleEventMapper();
    }

    [Fact]
    public void Map_Should_ReturnProjectCreatedIntegrationEvent_When_ProjectCreatedDomainEventProvided()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        ProjectCreatedDomainEvent domainEvent = new(projectId, "TST");

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().NotBeNull();
        ProjectCreatedIntegrationEvent integrationEvent = result.Should().BeOfType<ProjectCreatedIntegrationEvent>().Subject;
        integrationEvent.ProjectId.Should().Be(projectId);
        integrationEvent.Abbreviation.Should().Be("TST");
    }

    [Fact]
    public void Map_Should_ReturnProjectDeletedIntegrationEvent_When_ProjectDeletedDomainEventProvided()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        ProjectDeletedDomainEvent domainEvent = new(projectId);

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().NotBeNull();
        ProjectDeletedIntegrationEvent integrationEvent = result.Should().BeOfType<ProjectDeletedIntegrationEvent>().Subject;
        integrationEvent.ProjectId.Should().Be(projectId);
    }

    [Fact]
    public void Map_Should_ReturnFieldCreatedIntegrationEvent_When_FieldCreatedDomainEventProvided()
    {
        // Arrange
        Guid fieldId = Guid.NewGuid();
        Guid fieldTypeId = Guid.NewGuid();
        List<Guid> workItemTypeIds = [Guid.NewGuid()];
        FieldCreatedDomainEvent domainEvent = new(fieldId, "Priority", true, fieldTypeId, workItemTypeIds);

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().NotBeNull();
        FieldCreatedIntegrationEvent integrationEvent = result.Should().BeOfType<FieldCreatedIntegrationEvent>().Subject;
        integrationEvent.FieldId.Should().Be(fieldId);
        integrationEvent.Name.Should().Be("Priority");
        integrationEvent.IsOptional.Should().BeTrue();
        integrationEvent.FieldTypeId.Should().Be(fieldTypeId);
        integrationEvent.WorkItemTypeIds.Should().BeEquivalentTo(workItemTypeIds);
    }

    [Fact]
    public void Map_Should_ReturnFieldUpdatedIntegrationEvent_When_FieldUpdatedDomainEventProvided()
    {
        // Arrange
        Guid fieldId = Guid.NewGuid();
        Guid fieldTypeId = Guid.NewGuid();
        FieldUpdatedDomainEvent domainEvent = new(fieldId, "Priority", false, fieldTypeId, []);

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().NotBeNull();
        FieldUpdatedIntegrationEvent integrationEvent = result.Should().BeOfType<FieldUpdatedIntegrationEvent>().Subject;
        integrationEvent.FieldId.Should().Be(fieldId);
        integrationEvent.Name.Should().Be("Priority");
    }

    [Fact]
    public void Map_Should_ReturnFieldDeletedIntegrationEvent_When_FieldDeletedDomainEventProvided()
    {
        // Arrange
        Guid fieldId = Guid.NewGuid();
        FieldDeletedDomainEvent domainEvent = new(fieldId);

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().NotBeNull();
        FieldDeletedIntegrationEvent integrationEvent = result.Should().BeOfType<FieldDeletedIntegrationEvent>().Subject;
        integrationEvent.FieldId.Should().Be(fieldId);
    }

    [Fact]
    public void Map_Should_ReturnProjectInvitationCreatedIntegrationEvent_When_ProjectInvitationCreatedDomainEventProvided()
    {
        // Arrange
        Guid invitationId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Guid inviteeId = Guid.NewGuid();
        ProjectInvitationCreatedDomainEvent domainEvent = new(invitationId, projectId, inviteeId);

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().NotBeNull();
        ProjectInvitationCreatedIntegrationEvent integrationEvent =
            result.Should().BeOfType<ProjectInvitationCreatedIntegrationEvent>().Subject;
        integrationEvent.InvitationId.Should().Be(invitationId);
        integrationEvent.ProjectId.Should().Be(projectId);
        integrationEvent.InviteeId.Should().Be(inviteeId);
    }

    [Fact]
    public void Map_Should_ReturnProjectInvitationCancelledIntegrationEvent_When_ProjectInvitationCancelledDomainEventProvided()
    {
        // Arrange
        Guid invitationId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Guid inviteeId = Guid.NewGuid();
        ProjectInvitationCancelledDomainEvent domainEvent = new(invitationId, projectId, inviteeId);

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().NotBeNull();
        ProjectInvitationCancelledIntegrationEvent integrationEvent =
            result.Should().BeOfType<ProjectInvitationCancelledIntegrationEvent>().Subject;
        integrationEvent.InvitationId.Should().Be(invitationId);
        integrationEvent.ProjectId.Should().Be(projectId);
        integrationEvent.InviteeId.Should().Be(inviteeId);
    }

    [Fact]
    public void Map_Should_ReturnProjectMemberJoinedIntegrationEvent_When_ProjectMemberJoinedDomainEventProvided()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        IReadOnlyCollection<ProjectPermissionEntry> permissions =
        [
            new ProjectPermissionEntry("project.view", 0)
        ];
        ProjectMemberJoinedDomainEvent domainEvent = new(projectId, userId, permissions);

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().NotBeNull();
        ProjectMemberJoinedIntegrationEvent integrationEvent =
            result.Should().BeOfType<ProjectMemberJoinedIntegrationEvent>().Subject;
        integrationEvent.ProjectId.Should().Be(projectId);
        integrationEvent.UserId.Should().Be(userId);
        integrationEvent.Permissions.Should().HaveCount(1);
        integrationEvent.Permissions.First().PermissionName.Should().Be("project.view");
    }

    [Fact]
    public void Map_Should_ReturnProjectPermissionActionRequestedIntegrationEvent_When_ProjectPermissionActionRequestedDomainEventProvided()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid requestedByUserId = Guid.NewGuid();
        ProjectPermissionActionRequestedDomainEvent domainEvent =
            new(projectId, requestedByUserId, "project.roles", "CreateRole", "{\"name\":\"Admin\"}");

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().NotBeNull();
        ProjectPermissionActionRequestedIntegrationEvent integrationEvent =
            result.Should().BeOfType<ProjectPermissionActionRequestedIntegrationEvent>().Subject;
        integrationEvent.ProjectId.Should().Be(projectId);
        integrationEvent.RequestedByUserId.Should().Be(requestedByUserId);
        integrationEvent.PermissionName.Should().Be("project.roles");
        integrationEvent.ActionName.Should().Be("CreateRole");
        integrationEvent.ActionPayload.Should().Be("{\"name\":\"Admin\"}");
    }

    [Fact]
    public void Map_Should_ReturnNull_When_UnknownDomainEventProvided()
    {
        // Arrange
        UnknownDomainEvent domainEvent = new();

        // Act
        IIntegrationEvent? result = _mapper.Map(domainEvent);

        // Assert
        result.Should().BeNull();
    }

    private sealed record UnknownDomainEvent : BuildingBlocks.Domain.Events.IDomainEvent;
}
