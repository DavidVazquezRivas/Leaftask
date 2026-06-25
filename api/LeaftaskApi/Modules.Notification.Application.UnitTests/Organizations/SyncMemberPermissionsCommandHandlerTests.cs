using FluentAssertions;
using Modules.Notification.Application.Organizations.RemoveMemberPermissions;
using Modules.Notification.Application.Organizations.SyncMemberPermissions;
using Modules.Notification.Domain.Entities.Approval.Permission;
using Modules.Notification.Domain.Repositories;
using NSubstitute;

namespace Modules.Notification.Application.UnitTests.Organizations;

public class SyncMemberOrganizationPermissionsCommandHandlerTests
{
    private readonly SyncMemberOrganizationPermissionsCommandHandler _handler;
    private readonly IOrganizationPermissionReadModelRepository _repositoryMock;

    public SyncMemberOrganizationPermissionsCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationPermissionReadModelRepository>();
        _handler = new SyncMemberOrganizationPermissionsCommandHandler(_repositoryMock);
    }

    [Fact]
    public async Task Handle_Should_DeleteExistingAndAddNewPermissions()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid orgId = Guid.NewGuid();
        OrganizationPermissionEntryDto[] permissions =
        [
            new("org.read", 1),
            new("org.write", 2)
        ];
        SyncMemberOrganizationPermissionsCommand command = new(orgId, userId, permissions);

        IEnumerable<OrganizationPermissionReadModel>? added = null;
        await _repositoryMock.AddRangeAsync(
            Arg.Do<IEnumerable<OrganizationPermissionReadModel>>(p => added = p),
            Arg.Any<CancellationToken>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.Received(1).DeleteByMemberAsync(userId, orgId, Arg.Any<CancellationToken>());
        added.Should().HaveCount(2);
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

public class RemoveMemberOrganizationPermissionsCommandHandlerTests
{
    private readonly RemoveMemberOrganizationPermissionsCommandHandler _handler;
    private readonly IOrganizationPermissionReadModelRepository _repositoryMock;

    public RemoveMemberOrganizationPermissionsCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationPermissionReadModelRepository>();
        _handler = new RemoveMemberOrganizationPermissionsCommandHandler(_repositoryMock);
    }

    [Fact]
    public async Task Handle_Should_DeleteMemberPermissions()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid orgId = Guid.NewGuid();
        RemoveMemberOrganizationPermissionsCommand command = new(orgId, userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.Received(1).DeleteByMemberAsync(userId, orgId, Arg.Any<CancellationToken>());
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
