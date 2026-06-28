using Modules.Projects.Application.Agents.Delete;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Agents.Delete;

public class DeleteAgentReadModelOnAgentDeletedCommandHandlerTests
{
    private readonly DeleteAgentReadModelOnAgentDeletedCommandHandler _handler;
    private readonly IAgentReadModelRepository _agentReadModelRepositoryMock;
    private readonly IProjectMemberRepository _memberRepositoryMock;

    public DeleteAgentReadModelOnAgentDeletedCommandHandlerTests()
    {
        _agentReadModelRepositoryMock = Substitute.For<IAgentReadModelRepository>();
        _memberRepositoryMock = Substitute.For<IProjectMemberRepository>();

        _handler = new DeleteAgentReadModelOnAgentDeletedCommandHandler(
            _agentReadModelRepositoryMock,
            _memberRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_RemoveMemberAndAgent_When_AgentIsMember()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        DeleteAgentReadModelOnAgentDeletedCommand command = new(agentId, projectId);

        ProjectRole role = new(Guid.NewGuid(), "Agent Role", ProjectTestBuilder.AProject().Build());
        ProjectMember member = new(Guid.NewGuid(), agentId, MemberType.Agent, role, ProjectTestBuilder.AProject().Build());
        _memberRepositoryMock.GetByMemberIdTrackedAsync(projectId, agentId, Arg.Any<CancellationToken>())
            .Returns(member);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _memberRepositoryMock.Received(1).Remove(member);
        await _agentReadModelRepositoryMock.Received(1).RemoveByIdAsync(agentId, Arg.Any<CancellationToken>());
        await _memberRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_RemoveAgentOnly_When_AgentIsNotMember()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        DeleteAgentReadModelOnAgentDeletedCommand command = new(agentId, projectId);

        _memberRepositoryMock.GetByMemberIdTrackedAsync(projectId, agentId, Arg.Any<CancellationToken>())
            .Returns((ProjectMember?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _memberRepositoryMock.DidNotReceive().Remove(Arg.Any<ProjectMember>());
        await _agentReadModelRepositoryMock.Received(1).RemoveByIdAsync(agentId, Arg.Any<CancellationToken>());
        await _memberRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
