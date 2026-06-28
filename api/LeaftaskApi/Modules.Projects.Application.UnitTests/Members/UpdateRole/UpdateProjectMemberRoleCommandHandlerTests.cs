using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Members.UpdateRole;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Members.UpdateRole;

public class UpdateProjectMemberRoleCommandHandlerTests
{
    private readonly UpdateProjectMemberRoleCommandHandler _handler;
    private readonly IProjectMemberRepository _memberRepositoryMock;
    private readonly IProjectRoleRepository _roleRepositoryMock;

    public UpdateProjectMemberRoleCommandHandlerTests()
    {
        _memberRepositoryMock = Substitute.For<IProjectMemberRepository>();
        _roleRepositoryMock = Substitute.For<IProjectRoleRepository>();

        _handler = new UpdateProjectMemberRoleCommandHandler(
            _memberRepositoryMock,
            _roleRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RoleIsUpdated()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid memberId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        UpdateProjectMemberRoleCommand command = new(projectId, memberId, roleId);

        ProjectRole currentRole = new(Guid.NewGuid(), "Member", ProjectTestBuilder.AProject().Build());
        ProjectMember member = new(Guid.NewGuid(), memberId, MemberType.User, currentRole, ProjectTestBuilder.AProject().Build());
        _memberRepositoryMock.GetByMemberIdTrackedAsync(projectId, memberId, Arg.Any<CancellationToken>())
            .Returns(member);

        ProjectRole newRole = new(roleId, "Admin", ProjectTestBuilder.AProject().Build());
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns(newRole);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _memberRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_MemberNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid memberId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        UpdateProjectMemberRoleCommand command = new(projectId, memberId, roleId);

        _memberRepositoryMock.GetByMemberIdTrackedAsync(projectId, memberId, Arg.Any<CancellationToken>())
            .Returns((ProjectMember?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.MemberNotFound);
        await _memberRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoleNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid memberId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        UpdateProjectMemberRoleCommand command = new(projectId, memberId, roleId);

        ProjectRole currentRole = new(Guid.NewGuid(), "Member", ProjectTestBuilder.AProject().Build());
        ProjectMember member = new(Guid.NewGuid(), memberId, MemberType.User, currentRole, ProjectTestBuilder.AProject().Build());
        _memberRepositoryMock.GetByMemberIdTrackedAsync(projectId, memberId, Arg.Any<CancellationToken>())
            .Returns(member);

        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns((ProjectRole?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.RoleNotFound);
        await _memberRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
