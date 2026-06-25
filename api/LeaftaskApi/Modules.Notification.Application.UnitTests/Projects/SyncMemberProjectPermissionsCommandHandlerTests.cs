using FluentAssertions;
using Modules.Notification.Application.Projects.SyncMemberPermissions;
using Modules.Notification.Domain.Entities.Approval.Permission;
using Modules.Notification.Domain.Repositories;
using NSubstitute;

namespace Modules.Notification.Application.UnitTests.Projects;

public class SyncMemberProjectPermissionsCommandHandlerTests
{
    private readonly SyncMemberProjectPermissionsCommandHandler _handler;
    private readonly IProjectPermissionReadModelRepository _repositoryMock;

    public SyncMemberProjectPermissionsCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IProjectPermissionReadModelRepository>();
        _handler = new SyncMemberProjectPermissionsCommandHandler(_repositoryMock);
    }

    [Fact]
    public async Task Handle_Should_DeleteExistingAndAddNewPermissions()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        ProjectPermissionEntryDto[] permissions =
        [
            new("proj.read", 1),
            new("proj.write", 2)
        ];
        SyncMemberProjectPermissionsCommand command = new(projectId, userId, permissions);

        IEnumerable<ProjectPermissionReadModel>? added = null;
        await _repositoryMock.AddRangeAsync(
            Arg.Do<IEnumerable<ProjectPermissionReadModel>>(p => added = p),
            Arg.Any<CancellationToken>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.Received(1).DeleteByMemberAsync(userId, projectId, Arg.Any<CancellationToken>());
        added.Should().HaveCount(2);
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
