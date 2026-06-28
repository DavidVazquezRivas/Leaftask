using Modules.Projects.Application.Agents.Create;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Agents.Create;

public class CreateAgentReadModelOnAgentCreatedCommandHandlerTests
{
    private readonly CreateAgentReadModelOnAgentCreatedCommandHandler _handler;
    private readonly IAgentReadModelRepository _agentReadModelRepositoryMock;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IProjectMemberRepository _memberRepositoryMock;
    private readonly IProjectRoleRepository _roleRepositoryMock;

    public CreateAgentReadModelOnAgentCreatedCommandHandlerTests()
    {
        _agentReadModelRepositoryMock = Substitute.For<IAgentReadModelRepository>();
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _memberRepositoryMock = Substitute.For<IProjectMemberRepository>();
        _roleRepositoryMock = Substitute.For<IProjectRoleRepository>();

        _handler = new CreateAgentReadModelOnAgentCreatedCommandHandler(
            _agentReadModelRepositoryMock,
            _projectRepositoryMock,
            _memberRepositoryMock,
            _roleRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_AddAgentReadModelAndMember_When_AgentDoesNotExistAndIsNotMember()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        CreateAgentReadModelOnAgentCreatedCommand command = new(agentId, "My Agent", projectId, roleId);

        _agentReadModelRepositoryMock.ExistsByIdAsync(agentId, Arg.Any<CancellationToken>()).Returns(false);
        _memberRepositoryMock.ExistsByMemberIdAsync(projectId, agentId, Arg.Any<CancellationToken>()).Returns(false);

        Project project = ProjectTestBuilder.AProject().Build();
        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);

        ProjectRole role = new(roleId, "Agent Role", project);
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>()).Returns(role);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _agentReadModelRepositoryMock.Received(1).AddAsync(
            Arg.Is<AgentReadModel>(m => m.Id == agentId && m.Name == "My Agent"),
            Arg.Any<CancellationToken>());
        await _memberRepositoryMock.Received(1).AddAsync(
            Arg.Is<ProjectMember>(m => m.MemberId == agentId && m.MemberType == MemberType.Agent),
            Arg.Any<CancellationToken>());
        await _agentReadModelRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotAddAgentReadModel_When_AgentAlreadyExists()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        CreateAgentReadModelOnAgentCreatedCommand command = new(agentId, "My Agent", projectId, roleId);

        _agentReadModelRepositoryMock.ExistsByIdAsync(agentId, Arg.Any<CancellationToken>()).Returns(true);
        _memberRepositoryMock.ExistsByMemberIdAsync(projectId, agentId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _agentReadModelRepositoryMock.DidNotReceive().AddAsync(Arg.Any<AgentReadModel>(), Arg.Any<CancellationToken>());
        await _memberRepositoryMock.DidNotReceive().AddAsync(Arg.Any<ProjectMember>(), Arg.Any<CancellationToken>());
        await _agentReadModelRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_FallBackToDefaultRole_When_SpecifiedRoleNotFound()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        Guid defaultRoleId = Guid.NewGuid();
        CreateAgentReadModelOnAgentCreatedCommand command = new(agentId, "My Agent", projectId, roleId);

        _agentReadModelRepositoryMock.ExistsByIdAsync(agentId, Arg.Any<CancellationToken>()).Returns(false);
        _memberRepositoryMock.ExistsByMemberIdAsync(projectId, agentId, Arg.Any<CancellationToken>()).Returns(false);

        Project project = ProjectTestBuilder.AProject().Build();
        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);

        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns((ProjectRole?)null);

        ProjectRole defaultRole = new(defaultRoleId, "Member", project);
        _roleRepositoryMock.GetDefaultMemberRoleAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(defaultRole);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _memberRepositoryMock.Received(1).AddAsync(
            Arg.Is<ProjectMember>(m => m.Role == defaultRole),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotAddMember_When_ProjectNotFound()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        CreateAgentReadModelOnAgentCreatedCommand command = new(agentId, "My Agent", projectId, roleId);

        _agentReadModelRepositoryMock.ExistsByIdAsync(agentId, Arg.Any<CancellationToken>()).Returns(false);
        _memberRepositoryMock.ExistsByMemberIdAsync(projectId, agentId, Arg.Any<CancellationToken>()).Returns(false);

        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((Project?)null);
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns((ProjectRole?)null);
        _roleRepositoryMock.GetDefaultMemberRoleAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((ProjectRole?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _memberRepositoryMock.DidNotReceive().AddAsync(Arg.Any<ProjectMember>(), Arg.Any<CancellationToken>());
        await _agentReadModelRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
