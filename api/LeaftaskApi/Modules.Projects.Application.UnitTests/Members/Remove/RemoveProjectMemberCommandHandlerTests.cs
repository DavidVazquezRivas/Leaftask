using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Members.Remove;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Members.Remove;

public class RemoveProjectMemberCommandHandlerTests
{
    private readonly RemoveProjectMemberCommandHandler _handler;
    private readonly IProjectMemberRepository _memberRepositoryMock;

    public RemoveProjectMemberCommandHandlerTests()
    {
        _memberRepositoryMock = Substitute.For<IProjectMemberRepository>();
        _handler = new RemoveProjectMemberCommandHandler(_memberRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_MemberIsRemoved()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid memberId = Guid.NewGuid();
        RemoveProjectMemberCommand command = new(projectId, memberId);

        ProjectRole role = new(Guid.NewGuid(), "Member", ProjectTestBuilder.AProject().Build());
        ProjectMember member = new(Guid.NewGuid(), memberId, MemberType.User, role, ProjectTestBuilder.AProject().Build());
        _memberRepositoryMock.GetByMemberIdTrackedAsync(projectId, memberId, Arg.Any<CancellationToken>())
            .Returns(member);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _memberRepositoryMock.Received(1).Remove(member);
        await _memberRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_MemberNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid memberId = Guid.NewGuid();
        RemoveProjectMemberCommand command = new(projectId, memberId);

        _memberRepositoryMock.GetByMemberIdTrackedAsync(projectId, memberId, Arg.Any<CancellationToken>())
            .Returns((ProjectMember?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.MemberNotFound);
        _memberRepositoryMock.DidNotReceive().Remove(Arg.Any<ProjectMember>());
        await _memberRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
